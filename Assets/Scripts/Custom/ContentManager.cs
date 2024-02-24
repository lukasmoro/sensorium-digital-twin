using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using System.Collections;

// Servo Events
// soft - on launching DONE
// hard - on emergency break
// left - on riding left DONE
// right - on riding right DONE
// release - on launching drive mode DONE
// tension - on exiting drive mode

// Haptic Events
// approaching far DONE 
// approaching close DONE 
// data in hand DONE
// steering at peak levels DONE
// heartbeat (build up) DONE
// confirmation after heartbeat DONE


public class ContentManager : MonoBehaviour
{   
    public HapticlabsManager hapticlabs;
    public LeapMotionManager leapmotion;
    public TCPIntegration tcp;
    public Simulation simulation;
    public SlimeSettings settings;
    public InteractionCollisionDetection colliderFront;
    public InteractionCollisionDetection colliderBuckle;
    public InteractionCollisionDetection colliderSide;
    public Animator animatorVirtualUI;
    public Animator animatorPhysicalUI;
    public TextMeshProUGUI rfidDisplay;
    public TextMeshProUGUI distanceDisplay;
    public TextMeshProUGUI servoDisplay;
    public TextMeshProUGUI hapticDisplay;
    public TextMeshProUGUI pulseDisplay;
    public TextMeshProUGUI poseDisplay;
    public TextMeshProUGUI handtrackingPositionDisplay;
    public TextMeshProUGUI handtrackingRotationDisplay;
    public TextMeshProUGUI highTonesDisplay;
    public TextMeshProUGUI midTonesDisplay;
    public TextMeshProUGUI lowTonesDisplay;
    public GameObject scannerIndicator;
    public GameObject distanceIndicator;
    public GameObject pulseIndicator;
    public GameObject servo1Indicator;
    public GameObject servo2Indicator;
    public GameObject servo3Indicator;
    public GameObject servo4Indicator;
    public GameObject vibrator1Indicator;
    public GameObject vibrator2Indicator;
    public GameObject leapPoseIndicator;
    public GameObject wristTransform;
    public Text tachometerText1;
    public Text tachometerText2;
    public RectTransform steeringAngleIndicator1a;
    public RectTransform steeringAngleIndicator1b;
    public RectTransform steeringAngleIndicator2a;
    public RectTransform steeringAngleIndicator2b;
    public AudioSource audioSource;
    public bool tagScanned = false;
    public bool drivingActive = false;
    private int currentSpeed = 0;

    void Start()
    {   

        hapticlabs = FindObjectOfType<HapticlabsManager>();
        leapmotion = FindObjectOfType<LeapMotionManager>();
        tcp.OnMessageReceived += HandleTCPMessage;
        SerialCommunication.OnNewMessageReceived.AddListener(HandleSerialMessage);
    }

    void Update()
    {   
        Material leapMaterial = leapPoseIndicator.GetComponent<Renderer>().material;
        Color grabPoseColor = Color.red;
        Color releasePoseColor = Color.green;
        
        if (tagScanned)
        {   
            if(leapmotion.grabDetected && colliderBuckle.collisionDetected) 
            {
                poseDisplay.text = "Handtracking Pose: Grab";
                
                hapticlabs.SetTrackNameElement3();
                hapticlabs.SendMessageToHapticlabs();
                hapticDisplay.text = "Haptic Event: Data in Hand";


                StartCoroutine(IndicatorOn(leapMaterial, grabPoseColor));
            }

            else if (leapmotion.releaseDetected && colliderFront.collisionDetected)
            {   
                animatorVirtualUI.SetBool("Open Collective Canvas", true);
                animatorPhysicalUI.SetBool("Open Collective Canvas", true);
                poseDisplay.text = "Handtracking Pose: Release";
                
                StartCoroutine(IndicatorOn(leapMaterial, releasePoseColor));
                StartCoroutine(UpdateTachometerRandom());
            }

            else if (leapmotion.releaseDetected && colliderSide.collisionDetected)
            {   
                hapticlabs.SetTrackNameElement4();
                hapticlabs.SendMessageToHapticlabs();
                hapticDisplay.text = "Haptic Event: Heartbeat";
                hapticlabs.SetTrackNameElement5();
                hapticlabs.SendMessageToHapticlabs();
                hapticDisplay.text = "Haptic Event: Confirmation";
                StartCoroutine(DrivingMode());
            }

            leapmotion.grabDetected = false;
            leapmotion.releaseDetected = false;
        }
    }

    // TCP messages from wearable
    void HandleTCPMessage(string tcpMessage)
    {
        string[] parts = tcpMessage.Split(new string[] { ": " }, StringSplitOptions.None);

        if (int.TryParse(parts[1].Trim(), out int pulseValue))
        {
            pulseDisplay.text = "Pulse Signal: " + pulseValue;
            StartCoroutine(EmissionControlCoroutine(pulseValue));
        }
    }

    // Serial handling bi-directional
    void HandleSerialMessage(string message)
    {
        string rfidPrefix = "Tag ID: ";
        string distancePrefix = "Distance: ";

        Material servo1Material = servo1Indicator.GetComponent<Renderer>().material;
        Material servo2Material = servo2Indicator.GetComponent<Renderer>().material;
        Material servo3Material = servo3Indicator.GetComponent<Renderer>().material;
        Material servo4Material = servo4Indicator.GetComponent<Renderer>().material;
        Color servoColor = Color.yellow;

        // RFID Triggers
        if (message.StartsWith(rfidPrefix))
        {
            string rfidMessage = message.Replace("0x", "");

            animatorVirtualUI.SetBool("Open Fade In", true);
            animatorPhysicalUI.SetBool("Open Fade In", true);

            Material scannerMaterial = scannerIndicator.GetComponent<Renderer>().material;
            Color scannerColor = Color.magenta;
            StartCoroutine(IndicatorOn(scannerMaterial, scannerColor));

            if (rfidMessage.Equals("Tag ID: 2F00FEC77C6A"))
            {   
                rfidDisplay.text = rfidMessage;
                tagScanned = true;
                SerialCommunication.SendSerialMessageArduino("soft");
                servoDisplay.text = "Servo Event: Soft";

                hapticlabs.SetTrackNameElement2();
                hapticlabs.SendMessageToHapticlabs();
                hapticDisplay.text = "Haptic Event: Welcome";

                StartCoroutine(IndicatorOn(servo1Material, servoColor));
                StartCoroutine(IndicatorOn(servo2Material, servoColor));
                StartCoroutine(IndicatorOn(servo3Material, servoColor));
                StartCoroutine(IndicatorOn(servo4Material, servoColor));
            }
            else if (rfidMessage.Equals("Tag ID: 1E00BD4223C2"))
            {   
                rfidDisplay.text = rfidMessage;
                tagScanned = true;

                SerialCommunication.SendSerialMessageArduino("soft");
                servoDisplay.text = "Servo Event: Soft";

                hapticlabs.SetTrackNameElement2();
                hapticlabs.SendMessageToHapticlabs();
                hapticDisplay.text = "Haptic Event: Welcome";

                StartCoroutine(IndicatorOn(servo1Material, servoColor));
                StartCoroutine(IndicatorOn(servo2Material, servoColor));
                StartCoroutine(IndicatorOn(servo3Material, servoColor));
                StartCoroutine(IndicatorOn(servo4Material, servoColor));
            }
        }

        // Distance Triggers
        if(message.StartsWith(distancePrefix))
        {   
            string distanceMessage = message;
            distanceDisplay.text = distanceMessage;
            Material distanceMaterial = distanceIndicator.GetComponent<Renderer>().material;
            string distancePart = message.Substring(distancePrefix.Length).Replace(" ", "").Trim();
            string distanceValue = new string(distancePart.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());

            if (float.TryParse(distanceValue, out float distance))
            {
                if (!tagScanned && distance > 200.0f && distance <= 500.0f)
                {   
                    hapticlabs.SetTrackNameElement0();
                    hapticlabs.SendMessageToHapticlabs();
                    hapticDisplay.text = "Haptic Event: Approaching 2m - 5m";

                    Color distance1Color = Color.red;
                    StartCoroutine(IndicatorOn(distanceMaterial, distance1Color));

                }
                else if (!tagScanned && distance > 0f && distance <= 200.0f)
                {
                    hapticlabs.SetTrackNameElement1();
                    hapticlabs.SendMessageToHapticlabs();
                    hapticDisplay.text = "Haptic Event: Approaching 0m - 2m";

                    Color distance2Color = Color.green;
                    StartCoroutine(IndicatorOn(distanceMaterial, distance2Color));
                }
            }
        }
    }

    // Driving mode
    IEnumerator DrivingMode()
    {   
        drivingActive = true;
        StopCoroutine(UpdateTachometerRandom()); 
        StartCoroutine(UpdateTachometerBasedOnRotation());

        SerialCommunication.SendSerialMessageArduino("release");
        servoDisplay.text = "Servo Event: Release";
        
        animatorVirtualUI.SetBool("Open Driving", true);
        animatorPhysicalUI.SetBool("Open Driving", true);



        while (true)
        {
            UpdateSteeringAngleIndicator();
            yield return null; 
        }

    }

    // Update emission of indicator based on event
    IEnumerator IndicatorOn(Material material, Color emissionColor)
    {   
        float duration = 0.5f;
        float maxEmission = 5.0f;

        if (material != null)
        {
            material.EnableKeyword("_EMISSION");

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float lerpFactor = Mathf.PingPong(t, duration / 2) / (duration / 2);
                Color finalColor = emissionColor * Mathf.LinearToGammaSpace(lerpFactor * maxEmission);
                material.SetColor("_EmissionColor", finalColor);
                yield return null;
            }
            material.SetColor("_EmissionColor", Color.black);
        }
    }

    // Update emission of indicator based on sensor value
    IEnumerator EmissionControlCoroutine(int sensorValue)
    {
        Material pulseMaterial = pulseIndicator.GetComponent<Renderer>().material;
        float emissionIntensity = MapSensorValueToEmissionIntensity(sensorValue);

        pulseMaterial.EnableKeyword("_EMISSION");
        Color emissionColor = Color.green;
        pulseMaterial.SetColor("_EmissionColor", emissionColor * Mathf.LinearToGammaSpace(emissionIntensity));

        yield return new WaitForSeconds(0.5f); 

        pulseMaterial.SetColor("_EmissionColor", Color.black);
    }
    float MapSensorValueToEmissionIntensity(int sensorValue)
    {
        float minEmission = 0.0f;
        float maxEmission = 5.0f;
        int minSensorValue = 700;
        int maxSensorValue = 770;

        return Mathf.Lerp(minEmission, maxEmission, (float)(sensorValue - minSensorValue) / (maxSensorValue - minSensorValue));
    }

    //Update speed random
    IEnumerator UpdateTachometerRandom()
    {   
        audioSource.Play();
        yield return new WaitForSeconds(3.30f); 
        
        while (true)
        {   
            StartCoroutine(UpdateSlimeSettingsFrequency());
            int targetSpeed = Random.Range(50, 170); 
            int speedChangeStep = targetSpeed > currentSpeed ? 1 : -1; 

            while (currentSpeed != targetSpeed)
            {
                currentSpeed += speedChangeStep; 
                tachometerText1.text = currentSpeed.ToString();
                tachometerText2.text = currentSpeed.ToString();
                yield return new WaitForSeconds(0.05f); 
            }
            yield return new WaitForSeconds(0.1f); 
         }
    }

    // Update speed through left hand rotation on y
    IEnumerator UpdateTachometerBasedOnRotation()
    {
        while (true)
        {
            float rotationValue = wristTransform.transform.localEulerAngles.y;
            handtrackingRotationDisplay.text = "Handtracking Rotation Left: " + Mathf.Round(rotationValue * 100f) / 100f;
            
            if (rotationValue > 180) rotationValue -= 360;

            float normalizedRotation = NormalizeRotation(rotationValue);
            currentSpeed = MapRotationToSpeed(normalizedRotation);

            tachometerText1.text = currentSpeed.ToString();
            tachometerText2.text = currentSpeed.ToString();

            yield return null;
        }
    }

    float NormalizeRotation(float rotationValue)
    {
        rotationValue = Mathf.Clamp(rotationValue, -60, 95);
        return (rotationValue + 60) / (95 + 60);
    }

    int MapRotationToSpeed(float normalizedRotation)
    {
        return Mathf.RoundToInt(normalizedRotation * 170);
    }

    // Update steering angle through left hand transform on x
    void UpdateSteeringAngleIndicator()
    {
        float transformationValue = wristTransform.transform.localPosition.x;
        handtrackingPositionDisplay.text = "Handtracking Position Left: " + Mathf.Round(transformationValue * 100f) / 100f;
        transformationValue = Mathf.Clamp(transformationValue, 0.25f, 0.40f);
        float steeringAngle = MapTransformationToSteeringAngle(transformationValue);

        if (steeringAngle <= -15f)
        {
            hapticlabs.SetTrackNameElement3();
            hapticlabs.SendMessageToHapticlabs();
            hapticDisplay.text = "Haptic Event: Extreme Curve Left";

            SerialCommunication.SendSerialMessageArduino("left");
            servoDisplay.text = "Servo Event: Left";
        } 
        else if (steeringAngle >= 15f)
        {
            hapticlabs.SetTrackNameElement3();
            hapticlabs.SendMessageToHapticlabs();
            hapticDisplay.text = "Haptic Event: Extreme Curve Right";

            SerialCommunication.SendSerialMessageArduino("right");
            servoDisplay.text = "Servo Event: Right";
        }

        steeringAngleIndicator1a.localEulerAngles = new Vector3(0, 0, steeringAngle);
        steeringAngleIndicator2a.localEulerAngles = new Vector3(0, 0, steeringAngle);
        steeringAngleIndicator1b.localEulerAngles = new Vector3(0, 0, steeringAngle);
        steeringAngleIndicator2b.localEulerAngles = new Vector3(0, 0, steeringAngle);
    }

    float MapTransformationToSteeringAngle(float transformationValue)
    {
        float normalizedTransformation = (transformationValue - (0.25f)) / (0.4f - (0.25f));
        return Mathf.Lerp(-15f, 15f, normalizedTransformation);
    }


    // Update slime settings based on frequency
    IEnumerator UpdateSlimeSettingsFrequency()
    {
        while(true)
        {
            float newSensorAngleSpacing = CalculateSlimeSettings();
            settings.speciesSettings[0].sensorAngleSpacing = newSensorAngleSpacing;

            if (drivingActive && settings != null && settings.speciesSettings != null && settings.speciesSettings.Length > 0)
            {
                settings.speciesSettings[0].colour = new Color(0, 0, 0, 0);
            }


            yield return null;
        }
    }

    float CalculateSlimeSettings()
    {   
        float highToneValue = Mathf.Round(AudioPeer.audioBandBuffer[7]*100f) / 100f;
        highTonesDisplay.text = "High Frequency: " + highToneValue;

        float midToneValue = Mathf.Round(AudioPeer.audioBandBuffer[4]*100f) / 100f;
        midTonesDisplay.text = "Mid Frequency: " + midToneValue;

        float lowToneValue = Mathf.Round(AudioPeer.audioBandBuffer[0]*100f) / 100f;
        lowTonesDisplay.text = "Low Frequency: " + lowToneValue;
        
        if(!drivingActive)
        {
            return lowToneValue * 100f;
        }
        else
        {
            return 45;
        }
    }

    // Reset OnDestroy
    void OnDestroy()
    {
        if (tcp != null)
        {
            tcp.OnMessageReceived -= HandleTCPMessage;
        }

        settings.trailWeight = 12f;
        settings.speciesSettings[0].sensorAngleSpacing = 45f;
    }
}
