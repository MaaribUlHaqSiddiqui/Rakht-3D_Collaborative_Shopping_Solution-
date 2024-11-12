using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SFB;
using System.Collections;
using TMPro;
using System.IO;


public class ImageHandler : MonoBehaviour
{
    public RawImage rawImage;
    public GameObject photonManagerObject;
    public TMP_InputField input_height;

    // Function to open the file picker dialog
    public void PickImage()
    {
        // Set filters to allow only image files (you can customize this based on your requirements)
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "gif"),
            new ExtensionFilter("All Files", "*")
        };

        // Show file picker dialog
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        // Check if any file is selected
        if (paths.Length > 0)
        {
            LoadAndDisplayImage(paths[0]);
        }
    }

    // Function to load and display the selected image
    private void LoadAndDisplayImage(string path)
    {
        // Read the image bytes
        byte[] imageBytes = System.IO.File.ReadAllBytes(path);

        // Create a texture and load the image bytes
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Display the image in the RawImage component
        rawImage.texture = texture;
    }

    // Function to save the displayed image to a folder in the Assets directory
    public void SaveImage()
    {
        if (photonManagerObject != null)
        {
            PhotonManager photonManager = photonManagerObject.GetComponent<PhotonManager>();
            if (photonManager != null)
            {
                string username = photonManager.GetUsername();


                if (rawImage.texture != null)
                {
                    /*// Convert the texture to bytes (PNG format)
                    byte[] imageBytes = ((Texture2D)rawImage.texture).EncodeToPNG();

                    // Specify the path to save the image within the Assets folder
                    string savePath = "Assets/user_images/"+ username +"_image.png";

                    // Write the bytes to a file
                    System.IO.File.WriteAllBytes(savePath, imageBytes);
                    UnityEngine.Debug.Log("Image saved at: " + savePath);

                    // Now, send the image path to the server
                    StartCoroutine(SendImagePathToServer(username, savePath));*/

                    // Specify the folder path to save the image
                    string folderPath = @"C:\Users\Maari\Unity Projects\sample_project2\Assets\user_images\";

                    // Convert the texture to bytes (PNG format)
                    byte[] imageBytes = ((Texture2D)rawImage.texture).EncodeToPNG();

                    // Specify the path to save the image within the Assets folder
                    string savePath = Path.Combine(folderPath, $"{username}_image.png");

                    // Write the bytes to a file
                    System.IO.File.WriteAllBytes(savePath, imageBytes);
                    UnityEngine.Debug.Log("Image saved at: " + savePath);

                    // Now, send the image path to the server
                    StartCoroutine(SendImagePathToServer(username, savePath));

                }
                else
                {
                    UnityEngine.Debug.LogWarning("No image to save. Please pick an image first.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("manager error");
            }
        }
        else
        {
            UnityEngine.Debug.Log("object error");
        }
    }

    IEnumerator SendImagePathToServer(string username, string imagePath)
    {
        // Replace the URL with the actual URL of your save_image.php script
        string url = "http://localhost:5000/upload-image";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("imagePath", imagePath);

        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www.error != null)
            {
                UnityEngine.Debug.LogError("Error: " + www.error);
            }
            else
            {
                UnityEngine.Debug.Log("Server Response: " + www.text);

                if(www.text.Contains("Image path saved successfully") || www.text.Contains("Image path updated successfully"))
                {
                    SendToMeasurementsScript(imagePath, input_height.text, username);
                }
            }
        }
    }

    /*void SendToMeasurementsScript(string imagePath, string height)
    {
        // Replace the URL with the actual URL of your measurements_main.py script
        string measurementsUrl = "http://localhost:5000/calculate_measurements";

        // Create a dictionary to hold the data
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("imagePath", imagePath);
        data.Add("height", height);

        // Convert the dictionary to JSON
        string jsonData = JsonUtility.ToJson(data);

        UnityEngine.Debug.Log("JSON Data: " + jsonData);

        // Set the Content-Type header to application/json
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        // Send the POST request with JSON data
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityEngine.Debug.Log("JSON Data: " + postData);

        WWW www = new WWW(measurementsUrl, postData, headers);

        StartCoroutine(WaitForMeasurementsResponse(www));
    }

    IEnumerator WaitForMeasurementsResponse(WWW www)
    {
        yield return www;

        if (www.error != null)
        {
            UnityEngine.Debug.LogError("Error sending data to /calculate_measurements: " + www.error);
        }
        else
        {
            UnityEngine.Debug.Log("Measurements Server Response: " + www.text);
            // Handle the response from /calculate_measurements as needed
        }
    }
    */



    void SendToMeasurementsScript(string imagePath, string height, string username)
    {
        // Replace the URL with the actual URL of your measurements_main.py script
        string measurementsUrl = "http://localhost:5000/calculate_measurements";

        WWWForm measurementsForm = new WWWForm();
        measurementsForm.AddField("imagePath", imagePath);
        measurementsForm.AddField("height", height);
        measurementsForm.AddField("username", username);

        StartCoroutine(WaitForMeasurementsResponse(measurementsUrl, measurementsForm));
    }

    IEnumerator WaitForMeasurementsResponse(string url, WWWForm form)
    {
        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www != null && www.error != null)
            {
                UnityEngine.Debug.LogError("Error sending data to measurements_main.py: " + www.error);
            }
            else if (www != null)
            {
                UnityEngine.Debug.Log("Measurements Server Response: " + www.text);
                // Handle the response from measurements_main.py as needed
            }
            else
            {
                UnityEngine.Debug.LogError("www is null.");
            }
        }
    }


    }
