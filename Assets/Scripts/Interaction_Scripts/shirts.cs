using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class shirts : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject panelToShow;
    private string _prompt;

    public string Interaction_prompt => _prompt;

    //[SerializeField] private string shirtPrefabPath = "Assets/Prefabs/Image (9).prefab";
    [Header("Shirt Prefabl")]
    public Transform contentContainer;
    public GameObject shirtPrefab;

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    public bool Interact(Interaction interactor)
    {
        Debug.Log(message: "viewing shirts");
        if (panelToShow != null)
        {
            StartCoroutine(FetchShirtData());
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

    IEnumerator FetchShirtData()
    {
        string url = "http://localhost:6000/get-shirts";

        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error fetching data: " + www.error);
            yield break;
        }

        string jsonData = www.text;
        Debug.Log($"{jsonData}");
        ShirtDataCollection shirtdata2;
        ShirtData[] shirtdata;

        try
        {
            //shirtdata = JsonUtility.FromJson<ShirtDataCollection>(jsonData); // Deserialize into ShirtData array
            shirtdata = JsonConvert.DeserializeObject<ShirtData[]>(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing JSON: " + e.Message);
            yield break;
        }

        if (shirtdata != null)
        {
            string jsonData2 = JsonUtility.ToJson(shirtdata);
            Debug.Log("Shirt Data Collection (JSON):");
            Debug.Log(jsonData2);
        }
        else
        {
            Debug.Log("shirt data empty");
        }
        

        // Access and process the fetched data here (replace with your logic)
        foreach (ShirtData shirt in shirtdata)
        {
            Debug.Log("Image Path: " + shirt.imagePath);
            Debug.Log("Length: " + shirt.height);
            Debug.Log("Width: " + shirt.width);
            Debug.Log("Neck: " + shirt.neck);
            Debug.Log("Shoulder: " + shirt.shoulder);

            /*GameObject prefab = Resources.Load<GameObject>(shirtPrefabPath);
            if (prefab == null)
            {
                Debug.LogError("Failed to load shirt prefab!");
                yield break;
            }
            */

            GameObject shirtInstance = Instantiate(shirtPrefab, contentContainer.transform);
            instantiatedPrefabs.Add(shirtInstance);

            RawImage rawImage = shirtInstance.transform.Find("RawImage").GetComponent<RawImage>();

            //string rootPath = @"C:\Users\Maari\Unity Projects\sample_project2\Assets\Shirts_images\";

            //string fullPath = Path.Combine(rootPath, shirt.imagePath);

            if (File.Exists(shirt.imagePath))
            {
                // Read the image bytes
                byte[] imageBytes = System.IO.File.ReadAllBytes(shirt.imagePath);

                // Create a texture and load the image bytes
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);

                // Display the image in the RawImage component
                rawImage.texture = texture;
            }



            TMP_Text heightText = shirtInstance.transform.Find("GameObject/HeightText").GetComponent<TMP_Text>();
            TMP_Text widthText = shirtInstance.transform.Find("GameObject/WidthText").GetComponent<TMP_Text>();
            TMP_Text neckText = shirtInstance.transform.Find("GameObject/NeckText").GetComponent<TMP_Text>();
            TMP_Text shoulderText = shirtInstance.transform.Find("GameObject/ShoulderText").GetComponent<TMP_Text>();

            heightText.text = "Height: " + shirt.height.ToString();
            widthText.text = "Width: " + shirt.width.ToString();
            neckText.text = "Neck: " + shirt.neck.ToString();
            shoulderText.text = "Shoulder: " + shirt.shoulder.ToString();

            //StartCoroutine(FetchUserMeasurements(shirt));
            StartCoroutine(FetchUserMeasurements(shirt, recommendation =>
            {
                // Update the UI with the recommendation
                TMP_Text recommendationText = shirtInstance.transform.Find("RecommendationText").GetComponent<TMP_Text>();
                recommendationText.text = "Recomendation: " + recommendation;
            }));

        }

        int prefabCount = instantiatedPrefabs.Count;
        Debug.Log("Number of instantiated prefabs: " + prefabCount);


    }

    private void ClearInstantiatedPrefabs()
    {
        int prefabCount = instantiatedPrefabs.Count;
        Debug.Log("Number of instantiated prefabs: " + prefabCount);

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


    IEnumerator FetchUserMeasurements(ShirtData shirt, System.Action<string> callback)
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
            string recommendation = GenerateRecommendation(shirt, userMeasurements);
            Debug.Log("Recommendation: " + recommendation);

            // Pass the recommendation to the callback function
            callback?.Invoke(recommendation);

        }
        else
        {
            Debug.Log("user measurements data empty");
        }

    }

    /*string GenerateRecommendation(ShirtData shirt, UserMeasurements userMeasurements)
    {
        // Your recommendation logic here, using shirt and userMeasurements data
        // For example, you can compare shirt measurements with user measurements and suggest adjustments

        // Placeholder recommendation for demonstration
        return "Adjust shirt size for better fit";
    }*/

    string GenerateRecommendation(ShirtData shirt, UserMeasurements userMeasurements)
    {
        // Compare shirt measurements with user measurements and generate recommendation
        string recommendation = "Shirt fits well";

        // Check if shirt dimensions match user measurements
        bool shoulderFit = IsWithinTolerance(float.Parse(userMeasurements.distance_shoulder), shirt.shoulder);
        bool widthFit = IsWithinTolerance(float.Parse(userMeasurements.distance_waist), shirt.width);
        bool heightFit = IsWithinTolerance(float.Parse(userMeasurements.distance_arms), shirt.height);

        // If all dimensions match within tolerance, recommend the shirt fits well
        if (shoulderFit && widthFit && heightFit)
        {
            recommendation = "Shirt fits well";
        }
        else
        {
            // Generate recommendation based on specific mismatches
            List<string> adjustments = new List<string>();

            if (!shoulderFit)
            {
                adjustments.Add(shirt.shoulder < float.Parse(userMeasurements.distance_shoulder) ? "Shoulder is smaller" : "Shoulder is larger");
            }
            if (!widthFit)
            {
                adjustments.Add(shirt.width < float.Parse(userMeasurements.distance_waist) ? "Width is smaller" : "Width is larger");
            }
            if (!heightFit)
            {
                adjustments.Add(shirt.height < float.Parse(userMeasurements.distance_arms) ? "Height is smaller" : "Height is larger");
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




    /*string GenerateRecommendation(ShirtData shirt, UserMeasurements userMeasurements)
    {
        // Convert measurements to vectors
        Vector3 shirtVector = new Vector3(shirt.height, shirt.width, shirt.shoulder);
        Vector3 userVector = new Vector3(
            float.Parse(userMeasurements.distance_arms),
            float.Parse(userMeasurements.distance_waist),
            float.Parse(userMeasurements.distance_shoulder)
        );

        // Calculate cosine similarity
        float similarity = CosineSimilarity(shirtVector, userVector);

        // Generate recommendation based on similarity
        string recommendation = "";

        if (similarity > 0.9f)
        {
            recommendation = "Shirt  fit well but";

            // Detailed adjustments
            List<string> adjustments = new List<string>();

            if (Math.Abs(shirtVector.x - userVector.x) > 2)
            {
                adjustments.Add(shirtVector.x < userVector.x ? "Lenght is smaller" : "Lenght is larger");
            }
            if (Math.Abs(shirtVector.y - userVector.y) > 2)
            {
                adjustments.Add(shirtVector.y < userVector.y ? "Width is smaller" : "Width is larger");
            }
            if (Math.Abs(shirtVector.z - userVector.z) > 2)
            {
                adjustments.Add(shirtVector.z < userVector.z ? "Shoulder is smaller" : "Shoulder is larger");
            }

            if (adjustments.Count > 0)
            {
                recommendation += ": " + string.Join(", ", adjustments);
            }
        }
        else
        {
            // Detailed adjustments
            List<string> adjustments = new List<string>();

            if (Math.Abs(shirtVector.x - userVector.x) > 2)
            {
                adjustments.Add(shirtVector.x < userVector.x ? "Height is smaller" : "Height is larger");
            }
            if (Math.Abs(shirtVector.y - userVector.y) > 2)
            {
                adjustments.Add(shirtVector.y < userVector.y ? "Width is smaller" : "Width is larger");
            }
            if (Math.Abs(shirtVector.z - userVector.z) > 2)
            {
                adjustments.Add(shirtVector.z < userVector.z ? "Shoulder is smaller" : "Shoulder is larger");
            }

            if (adjustments.Count > 0)
            {
                recommendation += ": " + string.Join(", ", adjustments);
            }
        }


        return recommendation;
    }

    float CosineSimilarity(Vector3 vectorA, Vector3 vectorB)
    {
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        float magnitudeA = vectorA.magnitude;
        float magnitudeB = vectorB.magnitude;

        return dotProduct / (magnitudeA * magnitudeB);
    }*/



}

[Serializable]
public class ShirtData // Define a class to represent your shirt data structure
{
    public string imagePath;
    public int height;
    public int width;
    public int neck;
    public int shoulder;
}

[Serializable]
public class ShirtDataCollection
{
    public ShirtData[] shirtData;
}

[Serializable]
public class UserMeasurements
{
    public string userMeasurementID;
    public string userID;
    public string distance_shoulder;
    public string distance_waist;
    public string distance_arms;
    public string distance_arms2;
    public string distance_legs;
}
