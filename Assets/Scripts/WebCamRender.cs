using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class WebCamRender : MonoBehaviour
{
    public string deviceName;
    private string pic;   
    private string url = "https://ccd7eb7a-b301-4086-a09b-fdbfeff398f9-bluemix.cloudant.com/";
    private string db = "Enter Database name";
    private string username = "Enter your username here";
    private string password = "Enter your password here";
    private int nextPicture=10;
    private string device = "Dev1_";
    WebCamTexture wct;
    
   void Start () {
     WebCamDevice[] devices = WebCamTexture.devices;

    foreach(WebCamDevice cam in devices)
   {
     if(cam.isFrontFacing )
     {    
         wct =    new WebCamTexture(deviceName, 400, 300, 12);
         wct.deviceName  = cam.name;
         wct.Play();
     }
   }
}

     void Update () {
         if(Time.time>=nextPicture){
             nextPicture=Mathf.FloorToInt(Time.time)+10;
             TakeSnapshot();
         }
          }
    public void TakeSnapshot()
    {
        Texture2D snap = new Texture2D(wct.width, wct.height);
        snap.SetPixels(wct.GetPixels());
        snap.Apply();
        pic = System.Convert.ToBase64String(snap.EncodeToPNG());
        string dateTime = System.DateTime.Now.ToString("MMddyyyy-Hmmss");
       // System.IO.File.WriteAllText("PictureCapture.txt", pic);
        StartCoroutine(PutDoc(pic));

    }
    // Send picture to IBM Cloudant
    private IEnumerator PutDoc(string pic_string)
    {
        string dateTime = System.DateTime.Now.ToString("MMddyyyy-Hmmss");
        dateTime += device;
        string json = "{\"_id\":\"" + dateTime +"\", \"image\":\"" + pic_string + "\"}";

        Debug.Log("Put Image Request made:");
        // Request and wait for the desired page.

        var request = new UnityWebRequest(url + db, "POST");

        request.SetRequestHeader("Authorization", "Basic " + Base64Encode(username + ":" + password));

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();

        yield return null;

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
        }
    }
    public string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

} 