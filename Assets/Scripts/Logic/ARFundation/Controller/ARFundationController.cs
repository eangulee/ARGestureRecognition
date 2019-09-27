using com.baidu.ai;
using Common.Cameras;
using Logic.ResMgr;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFundationController : SingletonMono<ARFundationController>
{
    public delegate void GestureRecognitionHandler(bool success, ResultJson result);
    public GestureRecognitionHandler gestureRecognitionHandler;

    public delegate void GenerateTextureHandler(Texture2D texture2D);
    public GenerateTextureHandler generateTextureHandler;

    public const int MAX_ERROR_COUNT = 5;
    public const float TIME_OUT = 10f;

    private int _errorCount = 0;
    private float _lastTime = 0;
    public TokenJson tokenJson;

    public Camera captureCamera;
    public ARCameraManager arCameraManager;
    public XRCameraSubsystem xrCameraSubsystem;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        xrCameraSubsystem = arCameraManager.subsystem;
        string token = AccessToken.getAccessToken();
        Debug.Log("token:" + token);
        tokenJson = JsonUtility.FromJson<TokenJson>(token);
    }

    private Texture2D _texture;
    public Texture2D m_Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
            if (generateTextureHandler != null)
                generateTextureHandler(value);
        }
    }

    #region 帧绑定
    void Register()
    {
        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void UnRegister()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCameraImage image;
        if (!xrCameraSubsystem.TryGetLatestImage(out image))
            return;

        var conversionParams = new XRCameraImageConversionParams
        {
            // Get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            // Flip across the vertical axis (mirror image)
            //transformation = CameraImageTransformation.MirrorY
        };

        // See how many bytes we need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so we can dispose of the CameraImage. We must do this or it will leak resources.
        image.Dispose();

        // At this point, we could process the image, pass it to a computer vision algorithm, etc.
        // In this example, we'll just apply it to a texture to visualize it.

        // We've got the data; let's put it into a texture so we can visualize it.
        m_Texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        m_Texture.LoadRawTextureData(buffer);
        m_Texture.Apply();

        // Done with our temporary data
        buffer.Dispose();
        UnRegister();
    }
    #endregion

    public void GestureRecognition()
    {
        _errorCount = 0;
        GestureRecognitionInternal();
        StartCoroutine("TimeoutCoroutine");
    }

    private void GestureRecognitionInternal()
    {
        _lastTime = Time.time;
        if (_errorCount >= MAX_ERROR_COUNT)
        {
            if (gestureRecognitionHandler != null)
                gestureRecognitionHandler(false, null);
            StopCoroutine("TimeoutCoroutine");
            return;
        }

        Debug.LogFormat("第{0}次识别...", _errorCount + 1);
        //Register();
#if UNITY_EDITOR
        Debug.Log(ResUtil.GetSandBoxPath("1.png"));
        byte[] bytes = ResUtil.GetBytesFromLocal("1.png");
        ImageRecognition(bytes);
#else
            //GetARCameraImageAsync();
        GetImageAsync();
#endif
        //yield return new WaitForEndOfFrame();//拍照需要
        //byte[] bytes = PhotoGraphBytes();
        //byte[] bytes = GetARCameraImage();
        //if (bytes == null)
        //{
        //    yield return null;
        //    continue;
        //}

        //string result = com.baidu.ai.Gesture.gesture(tokenJson.access_token, bytes);
        //GestureJson gestureJson = JsonUtility.FromJson<GestureJson>(result);

        //Debug.Log("result:" + result);
        //if (gestureJson.result != null && gestureJson.result.Count > 0)
        //{
        //    foreach (var kvp in gestureJson.result)
        //    {
        //        string r = com.baidu.ai.Gesture.GetDescription(kvp.classname);
        //        Debug.Log(kvp.classname + "   " + r);
        //        if (!string.IsNullOrEmpty(r))
        //        {
        //            i = errorNum;
        //            StopCoroutine("GestureRecognitionCoroutine");
        //            if (gestureRecognitionHandler != null)
        //                gestureRecognitionHandler(true, r);
        //        }
        //    }
        //}
        //UnRegister();
    }

    private IEnumerator TimeoutCoroutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        while (Time.time - _lastTime < TIME_OUT)
        {
            yield return waitForSeconds;
        }
        if (gestureRecognitionHandler != null)
            gestureRecognitionHandler(false, null);
    }

    #region 异步加载
    public void GetImageAsync()
    {
        // Get information about the camera image
        XRCameraImage image;
        if (arCameraManager.TryGetLatestImage(out image))
        {
            // If successful, launch a coroutine that waits for the image
            // to be ready, then apply it to a texture.
            StartCoroutine(ProcessImage(image));

            // It is safe to dispose the image before the async operation completes.
            image.Dispose();
        }
    }

    IEnumerator ProcessImage(XRCameraImage image)
    {
        // Create the async conversion request
        var request = image.ConvertAsync(new XRCameraImageConversionParams
        {
            // Use the full image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 8, image.height / 8),

            // Color image format
            outputFormat = TextureFormat.RGB24,

            // Flip across the Y axis
            transformation = CameraImageTransformation.MirrorY
        });

        // Wait for it to complete
        while (!request.status.IsDone())
            yield return null;

        // Check status to see if it completed successfully.
        if (request.status != AsyncCameraImageConversionStatus.Ready)
        {
            // Something went wrong
            Debug.LogErrorFormat("Request failed with status {0}", request.status);

            // Dispose even if there is an error.
            request.Dispose();
            yield break;
        }

        // Image data is ready. Let's apply it to a Texture2D.
        var rawData = request.GetData<byte>();


        Texture2D texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false);

        // Copy the image data into the texture
        texture.LoadRawTextureData(rawData);
        texture.Apply();
        texture.Compress(false);

        m_Texture = Common.Util.TextureUtil.AnticlockwiseRotate90(texture);

        Debug.Log("生成图片！");

        byte[] bytes = m_Texture.EncodeToPNG();
        //SaveImage("temp", bytes);
        ImageRecognition(bytes);

        // Need to dispose the request to delete resources associated
        // with the request, including the raw data.
        request.Dispose();
    }
    #endregion

    #region 事件回调式
    public void GetARCameraImageAsync()
    {
        // Get information about the camera image
        XRCameraImage image;
        if (xrCameraSubsystem.TryGetLatestImage(out image))
        {
            // If successful, launch a coroutine that waits for the image
            // to be ready, then apply it to a texture.
            image.ConvertAsync(new XRCameraImageConversionParams
            {
                // Get the full image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                // Color image format
                outputFormat = TextureFormat.RGB24,

                // Flip across the Y axis
                //transformation = CameraImageTransformation.MirrorY

                // Call ProcessImage when the async operation completes
            }, ProcessImage);

            // It is safe to dispose the image before the async operation completes.
            image.Dispose();
        }
    }

    void ProcessImage(AsyncCameraImageConversionStatus status, XRCameraImageConversionParams conversionParams, NativeArray<byte> data)
    {
        if (status != AsyncCameraImageConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Async request failed with status {0}", status);
            return;
        }

        // Do something useful, like copy to a Texture2D or pass to a computer vision algorithm
        m_Texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);
        Debug.Log("生成图片！");
        m_Texture.LoadRawTextureData(data);
        m_Texture.Apply();

        byte[] bytes = m_Texture.EncodeToPNG();
        //SaveImage("temp", bytes);
        ImageRecognition(bytes);
        // data is destroyed upon return; no need to dispose
    }
    #endregion

    //图像识别
    void ImageRecognition(byte[] bytes)
    {
        string result = com.baidu.ai.Gesture.gesture(tokenJson.access_token, bytes);
        GestureJson gestureJson = JsonUtility.FromJson<GestureJson>(result);

        Debug.Log("result:" + result);
        if (gestureJson.result != null && gestureJson.result.Count > 0)
        {
            foreach (var kvp in gestureJson.result)
            {
                Debug.Log(kvp.ToString());
                if (!string.IsNullOrEmpty(com.baidu.ai.Gesture.GetDescription(kvp.classname)))//成功
                {
                    if (gestureRecognitionHandler != null)
                        gestureRecognitionHandler(true, kvp);
                    StopCoroutine("TimeoutCoroutine");
                }
                else
                {
                    _errorCount++;
                    GestureRecognitionInternal();//识别失败
                }
                break;//只取第一个结果
            }
        }
        else
        {
            _errorCount++;
            GestureRecognitionInternal();//识别失败
        }
    }

    //保存图片到本地
    void SaveImage(string fileName, byte[] bytes)
    {
        string filename = string.Format("{0}.png", fileName);
        ResUtil.DelRes(filename);
        ResUtil.Save2Local(filename, bytes);
    }

    //拍照截图
    public string PhotoGraph(string fileName)
    {
        string subPath = CameraPhotoGraph.SaveRenderTextureToPNG(fileName);
        return ResUtil.GetSandBoxPath(subPath);
    }

    //获取截图bytes
    public byte[] PhotoGraphBytes()
    {
        byte[] result = CameraPhotoGraph.SaveRenderTextureToPNGBytes();
        return result;
    }
}

