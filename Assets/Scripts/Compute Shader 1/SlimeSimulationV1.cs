using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct AgentV1
{
    public Vector2 position;
    public Vector2 direction;
    public float angleInRadians;

    public static int Size
    {
        get { return sizeof(float) * 2 * 2 + sizeof(float); }
    }
}

public class SlimeSimulationV1 : MonoBehaviour
{
    public ComputeShader computeShader;

    [Header("Texture")]
    [Range(128, 2048)]
    public int width;

    [Range(128, 2048)]
    public int height;

    [Header("General")]
    [RangeWithStep(32, 1048576, 32f)]
    public float numOfAgents;

    [RangeWithStep(0, 250, 10f)]
    public float distFromMapEdge;

    [Header("Speed")]
    [RangeWithStep(8, 256, 8f)]
    public float speed;

    [Header("Trail")]
    [RangeWithStep(0.0f, 1.5f, 0.1f)]
    public float diffuseRate;

    [RangeWithStep(0.1f, 1.0f, 0.1f)]
    public float trailDecayRate;

    [RangeWithStep(0.1f, 1.0f, 0.1f)]
    public float diffuseDecayRate;

    [Header("Sensor")]
    [RangeWithStep(0, 50, 2f)]
    public float sensorOffset;

    [RangeWithStep(0, 180, 5f)]
    public float sensorAngle;

    [RangeWithStep(0, 180, 5f)]
    public float rotationAngle;

    [Header("Color")]
    public Gradient gradient;

    public List<Gradient> gradients;

    [Header("Default Randomization Settings")]
    public bool randomizeSpeed;
    public bool randomizeTrail;
    public bool randomizeSensors;

    [Header("More Randomization Settings")]
    public bool resetAgents;
    public bool changeGradient;

    int currGradientIndex = 0;
    int nextGradientIndex = 0;
    float lerpDuration = 1;
    float lerpStartTime = 0;
    bool startGradientTransition = false;

    AgentV1[] agents;
    ComputeBuffer agentsBuffer;
    RenderTexture positionTexture;
    RenderTexture trailMapTexture;
    RenderTexture diffuseMapTexture;
    RenderTexture colorMapTexture;
    Texture2D gradientTexture;

    void Start()
    {
        // set depth to 0 for 2D textures
        int depth = 0;
        RenderTextureFormat format = RenderTextureFormat.ARGB32;
        RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;

        // create the position texture and enable UAV access
        positionTexture = new RenderTexture(width, height, depth, format, readWrite);
        positionTexture.enableRandomWrite = true;
        positionTexture.Create();

        trailMapTexture = new RenderTexture(width, height, depth, format, readWrite);
        trailMapTexture.enableRandomWrite = true;
        trailMapTexture.Create();

        diffuseMapTexture = new RenderTexture(width, height, depth, format, readWrite);
        diffuseMapTexture.enableRandomWrite = true;
        diffuseMapTexture.Create();

        colorMapTexture = new RenderTexture(width, height, depth, format, readWrite);
        colorMapTexture.enableRandomWrite = true;
        colorMapTexture.Create();

        ResetAgents();
        ResetGradientTexture();
    }

    public void RandomizeAgentSettings()
    {
        if (randomizeSpeed)
        {
            speed = UnityEngine.Random.Range(32, 192);
        }

        if (randomizeTrail)
        {
            diffuseRate = UnityEngine.Random.Range(0f, 1.5f);
            trailDecayRate = UnityEngine.Random.Range(0.1f, 1.0f);
            diffuseDecayRate = UnityEngine.Random.Range(0.1f, 1.0f);
        }

        if (randomizeSensors)
        {
            sensorOffset = UnityEngine.Random.Range(1, 50);
            sensorAngle = UnityEngine.Random.Range(1, 180);
            rotationAngle = UnityEngine.Random.Range(1, 180);
        }

        if (resetAgents)
        {
            ResetAgents();
        }

        if (changeGradient)
        {
            ChangeGradient();
        }
    }

    // note: for the youtube demo
    // public void ResetAgentsV0()
    // {
    //     int numOfAgentsInt = Mathf.RoundToInt(numOfAgents);
    //     agents = new AgentV1[numOfAgentsInt];
    //     for (int i = 0; i < numOfAgentsInt; i++)
    //     {
    //         float initialRadius = Mathf.Min(width, height) / 2 - distFromMapEdge;
    //         agents[i].position = initialRadius * UnityEngine.Random.insideUnitCircle;
    //         agents[i].direction = (Vector2.zero - agents[i].position).normalized;
    //         agents[i].angleInRadians = Mathf.Atan2(agents[i].direction.y, agents[i].direction.x);
    //     }
    //     agentsBuffer = new ComputeBuffer(numOfAgentsInt, AgentV1.Size);
    // }

    public void ResetAgents()
    {
        // set up a few agents to simulate
        int numOfAgentsInt = Mathf.RoundToInt(numOfAgents);
        agents = new AgentV1[numOfAgentsInt];
        for (int i = 0; i < numOfAgentsInt; i++)
        {
            // // part 1 - all agents at center facing outwards
            // agents[i].position = Vector2.zero;
            // agents[i].direction = UnityEngine.Random.insideUnitCircle.normalized;
            // agents[i].angleInRadians = Mathf.Atan2(agents[i].direction.y, agents[i].direction.x);

            // part 2 - circle facing inwards
            float initialRadius = Mathf.Min(width, height) / 2 - distFromMapEdge;
            agents[i].position = initialRadius * UnityEngine.Random.insideUnitCircle;
            agents[i].direction = (Vector2.zero - agents[i].position).normalized;
            // note: atan2 takes y first, then x
            // agents[i].angleInRadians = Mathf.Atan2(agents[i].direction.x, agents[i].direction.y);
            agents[i].angleInRadians = Mathf.Atan2(agents[i].direction.y, agents[i].direction.x);
        }

        // set up agentsBuffer to be the correct size
        agentsBuffer = new ComputeBuffer(numOfAgentsInt, AgentV1.Size);
    }

    void ResetGradientTexture()
    {
        int textureWidth = 256; // Set the desired width of the texture
        int textureHeight = 1; // Since it's a 1D gradient, set the height to 1

        if (gradientTexture == null)
        {
            gradientTexture = new Texture2D(
                textureWidth,
                textureHeight,
                TextureFormat.RGBA32,
                0,
                false
            );
        }

        for (int x = 0; x < textureWidth; x++)
        {
            float t = (float)x / (float)(textureWidth - 1); // Normalize x to [0, 1]
            Color color = gradient.Evaluate(t); // Evaluate the gradient color at position t
            gradientTexture.SetPixel(x, 0, color); // Set the color at the pixel position
        }

        gradientTexture.Apply();
    }

    public void ChangeGradient()
    {
        currGradientIndex = nextGradientIndex;
        nextGradientIndex = (nextGradientIndex + 1) % gradients.Count;

        startGradientTransition = true;
        lerpStartTime = Time.time;
    }

    void PerformGradientTransition()
    {
        if (startGradientTransition == false)
        {
            return;
        }

        float timeElapsed = Time.time - lerpStartTime;
        float lerpPercent = timeElapsed / lerpDuration;

        Gradient currGradient = gradients[currGradientIndex];
        Gradient nextGradient = gradients[nextGradientIndex];

        GradientColorKey[] currColorKeys = currGradient.colorKeys;
        GradientColorKey[] nextColorKeys = nextGradient.colorKeys;
        GradientColorKey[] finalColorKeys = gradient.colorKeys;
        for (int i = 0; i < finalColorKeys.Length; i++)
        {
            finalColorKeys[i].color = Color.Lerp(
                currColorKeys[i].color,
                nextColorKeys[i].color,
                lerpPercent
            );
            finalColorKeys[i].time = Mathf.Lerp(
                currColorKeys[i].time,
                nextColorKeys[i].time,
                lerpPercent
            );
        }

        // note: no need to transition alpha keys as they are always the same
        // GradientAlphaKey[] currAlphaKeys = currGrad.alphaKeys;
        // GradientAlphaKey[] nextAlphaKeys = nextGrad.alphaKeys;
        // GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        // for (int i = 0; i < alphaKeys.Length; i++)
        // {
        //     alphaKeys[i].alpha = Mathf.Lerp(
        //         currAlphaKeys[i].alpha,
        //         nextAlphaKeys[i].alpha,
        //         lerpPercent
        //     );
        //     alphaKeys[i].time = Mathf.Lerp(
        //         currAlphaKeys[i].time,
        //         nextAlphaKeys[i].time,
        //         lerpPercent
        //     );
        // }

        // Assign the modified keys back to the gradient
        gradient.SetKeys(finalColorKeys, gradient.alphaKeys);

        ResetGradientTexture();

        if (lerpPercent >= 1.0f)
        {
            startGradientTransition = false;
        }
    }

    public void RandomizeGradient()
    {
        if (gradient == null)
        {
            return;
        }
        GradientColorKey[] colorKeys = gradient.colorKeys;
        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].color = UnityEngine.Random.ColorHSV();
        }
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = 1;
        }
        gradient.SetKeys(colorKeys, alphaKeys);
        ResetGradientTexture();
    }

    void PrintAgentsPositions()
    {
        string[] arr = new string[agents.Length];
        for (int i = 0; i < agents.Length; i++)
        {
            arr[i] = agents[i].position.ToString();
        }
        Debug.Log(string.Join(", ", arr));
    }

    void Update()
    {
        if (agents == null)
        {
            return;
        }

        // DiscardContents() -> tells unity you no longer need this data.
        // it does not guarantee that memory is immediately released.
        // positionTexture.DiscardContents();

        // Release() -> tells unity immediately discard this memory
        // useful to reset previous positions value to float4(0, 0, 0, 0)
        // this texture should only store the current position of the agent
        positionTexture.Release();

        // set the "agents buffer" array with the latest position + direction data from "agents"
        agentsBuffer.SetData(agents);

        int kernelHandle1 = computeShader.FindKernel("CSPositionMap");
        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);
        computeShader.SetFloat("speed", speed);
        computeShader.SetFloat("distFromMapEdge", distFromMapEdge);

        computeShader.SetFloat("time", Time.time);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        computeShader.SetFloat("numOfAgents", numOfAgents);
        computeShader.SetFloat("sensorOffset", sensorOffset);
        computeShader.SetFloat("sensorAngle", sensorAngle);
        computeShader.SetFloat("rotationAngle", rotationAngle);

        computeShader.SetBuffer(kernelHandle1, "AgentsBuffer", agentsBuffer);
        computeShader.SetTexture(kernelHandle1, "PositionTexture", positionTexture);
        computeShader.SetTexture(kernelHandle1, "TrailMapTexture", trailMapTexture);
        computeShader.Dispatch(kernelHandle1, Mathf.RoundToInt(numOfAgents) / 32, 1, 1);

        // todo: figure out why we need to set the positionTexture again even though
        // we don't need to create the variable for #pragma kernel CSTrailMap
        int kernelHandle2 = computeShader.FindKernel("CSTrailMap");
        computeShader.SetFloat("trailDecayRate", trailDecayRate);
        computeShader.SetTexture(kernelHandle2, "PositionTexture", positionTexture);
        computeShader.SetTexture(kernelHandle2, "TrailMapTexture", trailMapTexture);
        computeShader.Dispatch(
            kernelHandle2,
            trailMapTexture.width / 8,
            trailMapTexture.height / 8,
            1
        );

        int kernelHandle3 = computeShader.FindKernel("CSDiffuseMap");
        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);
        computeShader.SetFloat("diffuseRate", diffuseRate);
        computeShader.SetFloat("diffuseDecayRate", diffuseDecayRate);
        computeShader.SetTexture(kernelHandle3, "PositionTexture", positionTexture);
        computeShader.SetTexture(kernelHandle3, "TrailMapTexture", trailMapTexture);
        computeShader.SetTexture(kernelHandle3, "DiffuseMapTexture", diffuseMapTexture);

        computeShader.SetTexture(kernelHandle3, "ColorMapTexture", colorMapTexture);
        computeShader.SetTexture(kernelHandle3, "GradientTexture", gradientTexture);

        computeShader.Dispatch(
            kernelHandle3,
            diffuseMapTexture.width / 8,
            diffuseMapTexture.height / 8,
            1
        );

        // copy diffuseTrailMapTexture into trailMapTexture so that the trailMap
        // can decrement the values in the blurred sections of the trail
        Graphics.Blit(diffuseMapTexture, trailMapTexture);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // meshRenderer.material.mainTexture = positionTexture;
        // meshRenderer.material.mainTexture = trailMapTexture;
        // meshRenderer.material.mainTexture = diffuseMapTexture;
        meshRenderer.material.mainTexture = colorMapTexture;

        // note: render scene to main camera
        // Camera mainCamera = Camera.main;
        // mainCamera.targetTexture = colorMapTexture;

        // update the "agents" array with the positions + directions from the compute shader
        agentsBuffer.GetData(agents);

        PerformGradientTransition();
    }

    void OnDestroy()
    {
        if (agentsBuffer == null)
        {
            return;
        }
        agentsBuffer.Release();
        Destroy(gradientTexture);
    }
}
