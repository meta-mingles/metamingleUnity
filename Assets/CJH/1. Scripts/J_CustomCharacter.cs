using Rukha93.ModularAnimeCharacter.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CustomCharacter : MonoBehaviour
{
    //
    FakeLoader fakeLoader;
    bool isMale = true;

    //기본 몸
    public GameObject[] body;

    [Header("Male")]
    //몸 구조
    public GameObject[] m_BasicBody;
    //머리카락의 부모
    public Transform m_TrBaseHead;
    //머리
    public SkinnedMeshRenderer m_Head;
    //상의
    public SkinnedMeshRenderer m_Top;
    public Material[] m_MatTops;
    //하의
    public SkinnedMeshRenderer m_Bottom;
    public Material[] m_MatBottom;
    //신발
    public SkinnedMeshRenderer m_Shoes;
    public Material[] m_MatShoes;

    [Header("Female")]
    //몸 구조
    public GameObject[] f_BasicBody;
    //머리카락의 부모
    public Transform f_TrBaseHead;
    //머리
    public SkinnedMeshRenderer f_Head;
    //상의
    public SkinnedMeshRenderer f_Top;
    public Material[] f_MatTops;
    //하의
    public SkinnedMeshRenderer f_Bottom;
    public Material[] f_MatBottom;
    //신발
    public SkinnedMeshRenderer f_Shoes;
    public Material[] f_MatShoes;

    async void Start()
    {
        fakeLoader = GetComponent<FakeLoader>();

        //load data
        KHHUserCustomData data = await KHHUserCustom.LoadData();

        KHHCategoryData categoryData;
        for (int i = 0; i < data.datas.Count; i++)
        {
            categoryData = data.datas[i];

            if (categoryData.category.Equals("body"))
            {
                isMale = categoryData.itemIndex == 0;
                body[0].SetActive(isMale);
                body[1].SetActive(!isMale);
            }
            else if (categoryData.category.Equals("head"))
            {
                m_Head.sharedMesh = fakeLoader.m_MaleItems.m_Heads[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                //head.material = matHeads[categoryData.itemIndex - 1];
                for (int j = 0; j < m_Head.materials.Length; j++)
                    SetColor(m_Head.materials[j], categoryData.materialDatas[j]); //SetColor(head.material, categoryData.materialDatas[0]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Heads[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("top") && categoryData.itemIndex > 0)
            {
                m_Top.sharedMesh = fakeLoader.m_MaleItems.m_Tops[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                m_Top.material = m_MatTops[categoryData.itemIndex - 1];
                for (int j = 0; j < m_Top.materials.Length; j++)
                    SetColor(m_Top.materials[j], categoryData.materialDatas[j]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Tops[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("bottom") && categoryData.itemIndex > 0)
            {
                m_Bottom.sharedMesh = fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                m_Bottom.material = m_MatBottom[categoryData.itemIndex - 1];
                for (int j = 0; j < m_Bottom.materials.Length; j++)
                    SetColor(m_Bottom.materials[j], categoryData.materialDatas[j]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("shoes") && categoryData.itemIndex > 0)
            {
                m_Shoes.sharedMesh = fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                m_Shoes.material = m_MatShoes[categoryData.itemIndex - 1];
                for (int j = 0; j < m_Shoes.materials.Length; j++)
                    SetColor(m_Shoes.materials[j], categoryData.materialDatas[j]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("hairstyle") && categoryData.itemIndex > 0)
            {
                for (int j = 0; j < fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects.Length; j++)
                {
                    GameObject hair = Instantiate(fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects[j].prefab, m_TrBaseHead);

                    Renderer[] mr = hair.transform.GetComponentsInChildren<Renderer>();
                    for (int k = 0; k < categoryData.materialDatas.Count; k++)
                    {
                        for (int l = 0; l < mr.Length; l++)
                        {
                            for (int m = 0; m < mr[l].materials.Length; m++)
                            {
                                string n = mr[l].materials[m].name.Replace(" (Instance)", "");

                                if (n.Equals(categoryData.materialDatas[k].name))
                                {
                                    SetColor(mr[l].materials[m], categoryData.materialDatas[k]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {

            }
        }
    }

    void SetColor(Material mat, KHHMaterialData matData)
    {
        if (mat.HasProperty("_MaskRemap"))
        {
            mat.SetColor("_Color_A_2", matData.ColorA);
            mat.SetColor("_Color_B_2", matData.ColorB);
            mat.SetColor("_Color_C_2", matData.ColorC);
        }
        else if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", matData.ColorA);
        }
    }


    void SetBasicBody(BodyPartType[] bodyPartTypes)
    {
        int bodyIdx;
        for (int i = 0; i < bodyPartTypes.Length; i++)
        {
            bodyIdx = (int)(bodyPartTypes[i]);
            if (BodyPartType.Arms_Lower <= bodyPartTypes[i] && bodyPartTypes[i] <= BodyPartType.Arms_Hand)
                bodyIdx -= 15;
            else if (BodyPartType.Legs_Upper <= bodyPartTypes[i] && bodyPartTypes[i] <= BodyPartType.Legs_Feet)
                bodyIdx -= 22;

            m_BasicBody[bodyIdx].SetActive(false);
        }

    }

    void Update()
    {

    }
}
