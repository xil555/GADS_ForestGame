using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class OllamaRequest 
{
    public string model;
    public string prompt;
    public bool stream;
}

[System.Serializable]
public class OllamaResponse
{
    public string model;
    public string created_at;
    public string response;
    public bool done;
}


public class OllamaManager : MonoBehaviour
{
    [Header("Ollama API Settings")]
    [Tooltip("The default local port for Ollama API is 11434")]
    private string ollamaUrl = "http://localhost:11434/api/generate";

    [Tooltip("Make sure you have pulled this model via the command prompt first")]
    public string modelName = "llama3";

    //basically sends a prompt to Ollama and returns the generated dialogue as a string
    public void GenerateDialogue(string systemPrompt, System.Action<string> onComplete)
    {
        StartCoroutine(SendRequestToOllama(systemPrompt, onComplete));
    }

    private IEnumerator SendRequestToOllama(string promptText, System.Action<string> onComplete)
    {

        OllamaRequest requestData = new OllamaRequest
        {
            model = modelName,
            prompt = promptText,
            stream = false // set to false so Ollama sends the complete sentence at once
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ollama Connection Error: " + request.error);
                onComplete?.Invoke("..."); // fallback text if the local server is down
            }
            else
            {
                // unpack the JSON response
                string jsonResponse = request.downloadHandler.text;
                OllamaResponse responseData = JsonUtility.FromJson<OllamaResponse>(jsonResponse);

                // send the generated text back to whatever script called it
                onComplete?.Invoke(responseData.response);
            }
        }

    }
}
    
    

    


    



    