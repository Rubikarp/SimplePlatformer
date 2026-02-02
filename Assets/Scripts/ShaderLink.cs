using UnityEngine;

public class ShaderLink : MonoBehaviour
{
    protected Renderer render = null;
    private MaterialPropertyBlock propBlock = null;

    protected virtual void Awake()
    {
        render = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        GetData(render, ref propBlock);
    }

    private void GetData(Renderer render, ref MaterialPropertyBlock propBlock)
    {
        if (render == null) GetComponent<Renderer>();

        //permet d'overide les param sans modif le mat ou créer d'instance
        propBlock = new MaterialPropertyBlock();
        //Recup Data
        render.GetPropertyBlock(propBlock, 0);
    }
    private void SendData() => render.SetPropertyBlock(propBlock, 0);

    public void UpdateProperty((string key, float value)[] shadersData)
    {
        GetData(render, ref propBlock);
        foreach (var shaderData in shadersData)
        {
            propBlock.SetFloat(shaderData.key, shaderData.value);
        }
        SendData();
    }
    public void UpdateProperty((string key, Texture2D value)[] shadersData)
    {
        GetData(render, ref propBlock); foreach (var shaderData in shadersData)
        {
            propBlock.SetTexture(shaderData.key, shaderData.value);
        }
        SendData();
    }
    public void UpdateProperty((string key, Vector4 value)[] shadersData)
    {
        GetData(render, ref propBlock); foreach (var shaderData in shadersData)
        {
            propBlock.SetVector(shaderData.key, shaderData.value);
        }
        SendData();
    }

    public void UpdateProperty((string key, float value) shaderData) => UpdateProperty(new (string, float)[1] { shaderData });
    public void UpdateProperty((string key, Texture2D value) shaderData) => UpdateProperty(new (string, Texture2D)[1] { shaderData });
    public void UpdateProperty((string key, Vector2 value) shaderData) => UpdateProperty(new (string, Vector4)[1] { shaderData });
    public void UpdateProperty((string key, Vector3 value) shaderData) => UpdateProperty(new (string, Vector4)[1] { shaderData });
    public void UpdateProperty((string key, Vector4 value) shaderData) => UpdateProperty(new (string, Vector4)[1] { shaderData });
}
