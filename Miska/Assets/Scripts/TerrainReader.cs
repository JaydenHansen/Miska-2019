using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainMaterial
{
    Grass,
    Dirt,
    Wood
};

public class TerrainReader : MonoBehaviour
{

    int m_alphaMapWidth, m_alphaMapHeight, m_numTextures;
    float[,,] m_splatMapData;
    TerrainData m_terrainData;
    public GameObject m_player;
    public StepSoundTrigger m_stepScript;

    private void Start()
    {
        m_terrainData = Terrain.activeTerrain.terrainData;
        m_alphaMapWidth = m_terrainData.alphamapWidth;
        m_alphaMapHeight = m_terrainData.alphamapHeight;

        m_splatMapData = m_terrainData.GetAlphamaps(0, 0, m_alphaMapWidth, m_alphaMapHeight);
        m_numTextures = m_splatMapData.Length / (m_alphaMapWidth * m_alphaMapHeight);
    }

    //Converts a global position into a 2D position (excl. y) on a texture map
    Vector3 ConvertToTerrainPos(Vector3 playerPos)
    {
        Vector3 pos = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPos = ter.transform.position;

        pos.x = ((playerPos.x - terPos.x) /  ter.terrainData.size.x) * m_alphaMapWidth;
        pos.z = ((playerPos.z - terPos.z) / ter.terrainData.size.z) * m_alphaMapHeight;

        return pos;
    }

    bool IsPlayerOnWood()
    {
        Vector3 rayOrigin = m_player.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit))
        {
            if (hit.collider.gameObject.tag == "WoodFloor")
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (IsPlayerOnWood())
        {
            m_stepScript.SetTerrainMaterial(TerrainMaterial.Wood);
        }
        else
        {
            TerrainMaterial terrainID = GetMainTexture();
            m_stepScript.SetTerrainMaterial(terrainID);
        }
        
        
    }

    //Creates an array of floats that defines the defines the relative amounts of different textures in the player's position
    float[] GetTextureMix()
    {
        Vector3 playerPos = m_player.transform.position;
        Vector3 terrPos = ConvertToTerrainPos(playerPos);

        float[,,] splatMap = m_terrainData.GetAlphamaps((int)terrPos.x, (int)terrPos.z, 1, 1);
        float[] cellMix = new float[splatMap.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatMap[0, 0, n];
        }

        return cellMix;
    }


    //Creates a Texture Mix array, and then selects the dominant texture
    public TerrainMaterial GetMainTexture()
    {
        float[] mix = GetTextureMix();
        float maxMix = 0.0f;
        int maxIndex = 0;

        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }

        if (maxIndex == 0 || maxIndex == 1 || maxIndex == 2 || maxIndex == 4 )
        {
            return TerrainMaterial.Grass;
        }
        else if (maxIndex == 3 || maxIndex == 5 || maxIndex == 6 || maxIndex == 7)
        {
            return TerrainMaterial.Dirt;
        }
        else
        {
            Debug.Log("ERROR! Terrain Reader failed to find matching Terrain Material, defaulting to Grass");
            return TerrainMaterial.Grass;
        }
    }
}
