using Photon.Pun;
using Rukha93.ModularAnimeCharacter.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CustomCharacter : MonoBehaviourPun, IPunObservable
{
    KHHUserCustomData data;
    string dataStr;

    FakeLoader fakeLoader;
    bool isMale = true;
    public Avatar[] avatars;

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
    //한벌옷
    public SkinnedMeshRenderer m_Outfit;
    public Material[] m_MatOutfit;

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
    //한벌옷
    public SkinnedMeshRenderer f_Outfit;
    public Material[] f_MatOutfit;

    async void Start()
    {

        dataStr = "";
        if (photonView.IsMine)
        {
            data = await KHHUserCustom.LoadData();
            SetAvatar();
            dataStr = JsonUtility.ToJson(data);
        }
    }

    void SetAvatar()
    {
        fakeLoader = GetComponent<FakeLoader>();
        KHHCategoryData categoryData;
        for (int i = 0; i < data.datas.Count; i++)
        {
            categoryData = data.datas[i];

            FakeLoader.ItemGroup itemGroup = isMale ? fakeLoader.m_MaleItems : fakeLoader.m_FemaleItems;
            if (categoryData.category.Equals("body"))
            {
                isMale = categoryData.itemIndex == 0;
                body[0].SetActive(isMale);
                body[1].SetActive(!isMale);
                GetComponent<Animator>().avatar = isMale ? avatars[0] : avatars[1];
            }
            else if (categoryData.category.Equals("head"))
            {
                SkinnedMeshRenderer head = isMale ? m_Head : f_Head;
                //head.material = matHeads[categoryData.itemIndex - 1];
                int catNum = 0;
                for (int j = 0; j < head.materials.Length; j++)
                {
                    if (categoryData.materialDatas.Count == 0) break;
                    if (head.materials[j].name.Contains(categoryData.materialDatas[catNum].name))
                    {
                        SetColor(head.materials[j], categoryData.materialDatas[catNum]);
                        catNum++;
                        if (catNum == categoryData.materialDatas.Count) break;
                    }
                }

                if (categoryData.itemIndex > 0)
                {
                    head.sharedMesh = itemGroup.m_Heads[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                    SetBasicBody(itemGroup.m_Heads[categoryData.itemIndex - 1].bodyParts);
                }
            }
            else if (categoryData.category.Equals("top") && categoryData.itemIndex > 0)
            {
                SkinnedMeshRenderer top = isMale ? m_Top : f_Top;
                top.sharedMesh = itemGroup.m_Tops[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                top.material = m_MatTops[categoryData.itemIndex - 1];
                int catNum = 0;
                for (int j = 0; j < top.materials.Length; j++)
                {
                    if (categoryData.materialDatas.Count == 0) break;
                    if (top.materials[j].name.Contains(categoryData.materialDatas[catNum].name))
                    {
                        SetColor(top.materials[j], categoryData.materialDatas[catNum]);
                        catNum++;
                        if (catNum == categoryData.materialDatas.Count) break;
                    }
                }
                SetBasicBody(itemGroup.m_Tops[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("bottom") && categoryData.itemIndex > 0)
            {
                SkinnedMeshRenderer bottom = isMale ? m_Bottom : f_Bottom;
                bottom.sharedMesh = itemGroup.m_Bottoms[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                bottom.material = m_MatBottom[categoryData.itemIndex - 1];
                int catNum = 0;
                for (int j = 0; j < bottom.materials.Length; j++)
                {
                    if (categoryData.materialDatas.Count == 0) break;
                    if (bottom.materials[j].name.Contains(categoryData.materialDatas[catNum].name))
                    {
                        SetColor(bottom.materials[j], categoryData.materialDatas[catNum]);
                        catNum++;
                        if (catNum == categoryData.materialDatas.Count) break;
                    }
                }
                SetBasicBody(itemGroup.m_Bottoms[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("shoes") && categoryData.itemIndex > 0)
            {
                SkinnedMeshRenderer shoes = isMale ? m_Shoes : f_Shoes;
                shoes.sharedMesh = itemGroup.m_Shoes[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                shoes.material = m_MatShoes[categoryData.itemIndex - 1];
                int catNum = 0;
                for (int j = 0; j < shoes.materials.Length; j++)
                {
                    if (categoryData.materialDatas.Count == 0) break;
                    if (shoes.materials[j].name.Contains(categoryData.materialDatas[catNum].name))
                    {
                        SetColor(shoes.materials[j], categoryData.materialDatas[catNum]);
                        catNum++;
                        if (catNum == categoryData.materialDatas.Count) break;
                    }
                }
                SetBasicBody(itemGroup.m_Shoes[categoryData.itemIndex - 1].bodyParts);
            }
            else if (categoryData.category.Equals("hairstyle") && categoryData.itemIndex > 0)
            {
                Transform baseHead = isMale ? m_TrBaseHead : f_TrBaseHead;
                for (int j = 0; j < itemGroup.m_Hairstyles[categoryData.itemIndex - 1].objects.Length; j++)
                {
                    GameObject hair = Instantiate(itemGroup.m_Hairstyles[categoryData.itemIndex - 1].objects[j].prefab, baseHead);

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
            else if (categoryData.category.Equals("outfit") && categoryData.itemIndex > 0)
            {
                SkinnedMeshRenderer outfit = isMale ? m_Outfit : f_Outfit;
                outfit.sharedMesh = itemGroup.m_Outfits[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                outfit.material = m_MatOutfit[categoryData.itemIndex - 1];
                int catNum = 0;
                for (int j = 0; j < outfit.materials.Length; j++)
                {
                    if (categoryData.materialDatas.Count == 0) break;
                    if (outfit.materials[j].name.Contains(categoryData.materialDatas[catNum].name))
                    {
                        SetColor(outfit.materials[j], categoryData.materialDatas[catNum]);
                        catNum++;
                        if (catNum == categoryData.materialDatas.Count) break;
                    }
                }
                SetBasicBody(itemGroup.m_Outfits[categoryData.itemIndex - 1].bodyParts);
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

            if (isMale)
                m_BasicBody[bodyIdx].SetActive(false);
            else
                f_BasicBody[bodyIdx].SetActive(false);
        }

    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(dataStr);
        }
        else
        {
            dataStr = (string)stream.ReceiveNext();
            data = JsonUtility.FromJson<KHHUserCustomData>(dataStr);
            SetAvatar();
        }
    }
}
