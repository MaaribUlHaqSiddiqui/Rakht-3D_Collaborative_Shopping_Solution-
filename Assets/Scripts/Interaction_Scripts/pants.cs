using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class pants : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject panelToShow;
    private string _prompt;

    public string Interaction_prompt => _prompt;

    //[SerializeField] private string shirtPrefabPath = "Assets/Prefabs/Image (9).prefab";
    [Header("Shirt Prefabl")]
    public Transform contentContainer;
    public GameObject pantPrefab;

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    public bool Interact(Interaction interactor)
    {
        Debug.Log(message: "viewing pants");
        if (panelToShow != null)
        {
            StartCoroutine(FetchPantData());
            panelToShow.SetActive(true);
        }
        else
        {
            Debug.Log(message: "cannot active panel");
        }
        return true;
    }

    public void DeactivatePanel()
    {
        // Deactivate the UI panel when the button is clicked
        if (panelToShow != null)
        {
            Debug.Log(message: "deactivbating panel");
            ClearInstantiatedPrefabs();
            panelToShow.SetActive(false);
        }
    }

    IEnumerator FetchPantData()
    {
        string url = "http://localhost:6000/get-pants";

        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error fetching data: " + www.error);
            yield break;
        }

        string jsonData = www.text;
        Debug.Log($"{jsonData}");
        PantDataCollection pantdata2;
        PantData[] pantdata;

        try
        {
            //shirtdata = JsonUtility.FromJson<ShirtDataCollection>(jsonData); // Deserialize into ShirtData array
            pantdata = JsonConvert.DeserializeObject<PantData[]>(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing JSON: " + e.Message);
            yield break;
        }

        if (pantdata != null)
        {
            string jsonData2 = JsonUtility.ToJson(pantdata);
            Debug.Log("Pant Data Collection (JSON):");
            Debug.Log(jsonData2);
        }
        else
        {
            Debug.Log("pant data empty");
        }


        // Access and process the fetched data here (replace with your logic)
        foreach (PantData pant in pantdata)
        {
            Debug.Log("Length: " + pant.length);
            Debug.Log("Width: " + pant.waist);

            /*GameObject prefab = Resources.Load<GameObject>(shirtPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("Failed to load shirt prefab!");
                yield break;
            }
            */

            GameObject pantInstance = Instantiate(pantPrefab, contentContainer.transform);
            instantiatedPrefabs.Add(pantInstance);

            RawImage rawImage = pantInstance.transform.Find("RawImage").GetComponent<RawImage>();

            if (File.Exists(pant.imagePath))
            {
                // Read the image bytes
                byte[] imageBytes = System.IO.File.ReadAllBytes(pant.imagePath);

                // Create a texture and load the image bytes
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);

                // Display the image in the RawImage component
                rawImage.texture = texture;
            }



            TMP_Text heightText = pantInstance.transform.Find("GameObject/HeightText").GetComponent<TMP_Text>();
            TMP_Text widthText = pantInstance.transform.Find("GameObject/WidthText").GetComponent<TMP_Text>();
            TMP_Text neckText = pantInstance.transform.Find("GameObject/NeckText").GetComponent<TMP_Text>();
            TMP_Text shoulderText = pantInstance.transform.Find("GameObject/ShoulderText").GetComponent<TMP_Text>();

            heightText.text = "Height: " + pant.length.ToString();
            widthText.text = "Width: " + pant.waist.ToString();
            //neckText.text = "Neck: " + pant.neck.ToString();
            //shoulderText.text = "Shoulder: " + pant.shoulder.ToString();

            //StartCoroutine(FetchUserMeasurements(shirt));
            StartCoroutine(FetchUserMeasurements(pant, recommendation =>
            {
                // Update the UI with the recommendation
                TMP_Text recommendationText = pantInstance.transform.Find("RecommendationText").GetComponent<TMP_Text>();
                recommendationText.text = "Recomendation: " + recommendation;
            }));

        }

    }

    private void ClearInstantiatedPrefabs()
    {
        int count = 0;
        // Destroy all instantiated prefabs (same as before)
        foreach (GameObject prefab in instantiatedPrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
                count++;
            }
        }
        Debug.Log("Total Prefabs: " + count);
        instantiatedPrefabs.Clear();
    }


    IEnumerator FetchUserMeasurements(PantData pant, System.Action<string> callback)
    {
        string playerName = PhotonNetwork.NickName;
        Debug.Log("Player Name: " + playerName);

        string url = "http://localhost:6000/get_user_measurements?username=" + playerName;

        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error fetching user measurements: " + www.error);
            yield break;
        }

        string jsonData = www.text;
        Debug.Log("User Measurements: " + jsonData);


        // Deserialize user measurements
        //UserMeasurements userMeasurements = JsonConvert.DeserializeObject<UserMeasurements>(jsonData);

        UserMeasurements userMeasurements;

        try
        {
            // Deserialize user measurements
            userMeasurements = JsonConvert.DeserializeObject<UserMeasurements>(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing JSON: " + e.Message);
            yield break;
        }

        if (userMeasurements != null)
        {
            string jsonData2 = JsonUtility.ToJson(userMeasurements);
            Debug.Log(jsonData2);
            Debug.Log("Shoulder: " + userMeasurements.distance_shoulder);

            // Compare shirt data with user measurements and generate recommendation
            string recommendation = GenerateRecommendation(pant, userMeasurements);
            Debug.Log("Recommendation: " + recommendation);

            // Pass the recommendation to the callback function
            callback?.Invoke(recommendation);

        }
        else
        {
            Debug.Log("user measurements data empty");
        }

    }

    /*string GenerateRecommendation(PantData pant, UserMeasurements userMeasurements)
    {
        // Your recommendation logic here, using shirt and userMeasurements data
        // For example, you can compare shirt measurements with user measurements and suggest adjustments

        // Placeholder recommendation for demonstration
        return "Adjust pant size for better fit";
    }*/

    string GenerateRecommendation(PantData pant, UserMeasurements userMeasurements)
    {
        // Compare pant measurements with user measurements and generate recommendation
        string recommendation;

        // Check if pant dimensions match user measurements
        bool waistFit = IsWithinTolerance(float.Parse(userMeasurements.distance_waist), pant.waist);
        bool lenghtFit = IsWithinTolerance(float.Parse(userMeasurements.distance_legs), pant.length);

        // If all dimensions match within tolerance, recommend the shirt fits well
        if (waistFit && lenghtFit)
        {
            recommendation = "Pant fits well";
        }
        else
        {
            // Generate recommendation based on specific mismatches
            List<string> adjustments = new List<string>();

            if (!waistFit)
            {
                adjustments.Add(pant.waist < float.Parse(userMeasurements.distance_waist) ? "Waist is smaller" : "Waist is larger");
            }
            if (!lenghtFit)
            {
                adjustments.Add(pant.length < float.Parse(userMeasurements.distance_legs) ? "Length is smaller" : "Length is larger");
            }

            // Combine adjustment recommendations into a single string
            recommendation = string.Join(", ", adjustments.ToArray());
        }

        return recommendation;
    }

    // Helper function to check if a measurement is within a tolerance range
    bool IsWithinTolerance(float measurement, int reference)
    {
        // Define your tolerance range here, for example, +/- 2
        int tolerance = 2;
        return Math.Abs(measurement - reference) <= tolerance;
    }


}

[Serializable]
public class PantData // Define a class to represent your shirt data structure
{
    public int length;
    public int waist;
    public string imagePath;

}

[Serializable]
public class PantDataCollection
{
    public PantData[] PantData;
}

[Serializable]
public class UserMeasurements2
{
    public string userMeasurementID;
    public string userID;
    public string distance_shoulder;
    public string distance_waist;
    public string distance_arms;
    public string distance_arms2;
    public string distance_legs;
}
