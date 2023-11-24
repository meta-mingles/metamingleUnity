using Rukha93.ModularAnimeCharacter.Customization;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHCustomCharacter : MonoBehaviour
{
    public static KHHCustomCharacter instance;
    public bool LoadCustom { get; set; }

    private IAssetLoader m_AssetLoader;

    private Animator m_Character;
    private SkinnedMeshRenderer m_ReferenceMesh;

    private Dictionary<string, List<string>> m_CustomizationOptions; //<categoryId, assetPath[]>

    private Dictionary<BodyPartType, BodyPartTag> m_BodyParts;
    private Dictionary<string, KHHCustomizationDemo.EquipedItem> m_Equiped;
    private Dictionary<Material, MaterialPropertyBlock> m_MaterialProperties;

    private Coroutine m_LoadingCoroutine;
    private void Awake()
    {
        instance = this;
        LoadCustom = false;
        //init variables
        m_AssetLoader = GetComponentInChildren<IAssetLoader>();
        m_CustomizationOptions = new Dictionary<string, List<string>>();
        m_Equiped = new Dictionary<string, KHHCustomizationDemo.EquipedItem>();
        m_BodyParts = new Dictionary<BodyPartType, BodyPartTag>();
        m_MaterialProperties = new Dictionary<Material, MaterialPropertyBlock>();
    }

    async void Start()
    {
        //load data
        KHHUserCustomData data = await KHHUserCustom.LoadData();
        StartCoroutine(InitLoadCustom(data));
    }

    IEnumerator InitLoadCustom(KHHUserCustomData data)
    {
        if (data == null || data.datas == null || data.datas.Count == 0)
        {
            m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody("f"));
            yield break;
        }

        for (int i = 0; i < data.datas.Count; i++)
        {
            var d = data.datas[i];
            if (d.category.Equals("body"))
            {
                m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody(d.itemIndex == 0 ? "m" : "f"));
            }
            else
            {
                m_LoadingCoroutine = StartCoroutine(Co_LoadAndEquip(d.category, m_CustomizationOptions[d.category][d.itemIndex], d.materialDatas));
            }
            yield return m_LoadingCoroutine;
        }
        LoadCustom = true;
    }

    private void InitBody(string path, GameObject prefab)
    {
        //instantiate the body prefab and store the animator
        prefab.gameObject.SetActive(true);
        m_Character = prefab.GetComponent<Animator>();

        //get a random body mesh to be used as reference
        var meshes = prefab.GetComponentsInChildren<SkinnedMeshRenderer>();
        m_ReferenceMesh = meshes[meshes.Length / 2];

        //initialize all tagged body parts
        //they be used to disable meshes that are hidden by clothes
        var bodyparts = m_Character.GetComponentsInChildren<BodyPartTag>();
        foreach (var part in bodyparts)
            m_BodyParts[part.type] = part;

        var equip = new KHHCustomizationDemo.EquipedItem()
        {
            path = path,
            assetReference = null,
            instantiatedObjects = new List<GameObject>() { m_Character.gameObject }
        };
        InitRenderersForItem(equip);
        m_Equiped["body"] = equip;
    }

    private IEnumerator Co_LoadAndInitBody(string bodyType)
    {
        //destroy old character
        if (m_Character != null)
        {
            Destroy(m_Character.gameObject);

            //clear old items
            m_Equiped.Clear();
            m_CustomizationOptions.Clear();
            m_BodyParts.Clear();
        }

        //init the customization options for the selected body type
        List<Coroutine> coroutines = new List<Coroutine>();
        for (int i = 0; i < KHHCustomizationDemo.CustomCategories.Count; i++)
        {
            int index = i;
            string path = KHHCustomizationDemo.CustomCategories[i].Equals("body") ? "body" : GetAssetPath(bodyType, KHHCustomizationDemo.CustomCategories[i]);
            coroutines.Add(StartCoroutine(m_AssetLoader.LoadAssetList(path, res => m_CustomizationOptions[KHHCustomizationDemo.CustomCategories[index]] = new List<string>(res))));
        }
        for (int i = 0; i < KHHCustomizationDemo.CustomCategories.Count; i++)
        {
            yield return coroutines[i];

            //add an empty item for all categories that can be empty
            if (KHHCustomizationDemo.CustomCategories[i].Equals("body"))
                continue;
            if (KHHCustomizationDemo.CustomCategories[i].Equals("head"))
                continue;
            m_CustomizationOptions[KHHCustomizationDemo.CustomCategories[i]].Insert(0, "");
        }

        //initialize the body
        var bodyPath = GetAssetPath(bodyType, "body");
        yield return m_AssetLoader.LoadAsset<GameObject>(bodyPath, res => InitBody(bodyPath, res));

        ////initialize the head with the first available
        //string assetPath = m_CustomizationOptions["head"][0];
        //yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(assetPath, res => Equip("head", assetPath, res));

        m_LoadingCoroutine = null;
    }
    private IEnumerator Co_LoadAndEquip(string cat, string path, List<KHHMaterialData> materialDatas = null)
    {
        yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(path, res => Equip(cat, path, res, materialDatas));
        m_LoadingCoroutine = null;
    }

    public void Equip(string cat, string path, CustomizationItemAsset item, List<KHHMaterialData> materialDatas = null)
    {
        if (string.IsNullOrEmpty(path) || item == null) return;
        //if outfit, remove all othet pieces

        ////unequip previous item
        //Unequip(cat, false);

        KHHCustomizationDemo.EquipedItem equip = new KHHCustomizationDemo.EquipedItem()
        {
            path = path,
            assetReference = item,
            instantiatedObjects = new List<GameObject>()
        };
        m_Equiped[cat] = equip;

        //instantiate new meshes, init properties, parent to character
        GameObject go = null;
        SkinnedMeshRenderer skinnedMesh = null;
        foreach (var mesh in item.meshes)
        {
            //instantiate new gameobject
            go = new GameObject(mesh.name);
            go.transform.SetParent(m_Character.transform, false);
            go.layer = m_Character.gameObject.layer;
            foreach (Transform child in go.transform)
                child.gameObject.layer = m_Character.gameObject.layer;
            m_Equiped[cat].instantiatedObjects.Add(go);

            //add the renderer
            skinnedMesh = go.AddComponent<SkinnedMeshRenderer>();
            skinnedMesh.rootBone = m_ReferenceMesh.rootBone;
            skinnedMesh.bones = m_ReferenceMesh.bones;
            skinnedMesh.bounds = m_ReferenceMesh.bounds;
            skinnedMesh.sharedMesh = mesh.sharedMesh;
            skinnedMesh.sharedMaterials = mesh.sharedMaterials;
        }

        //instantiate objects, parent to target bones
        foreach (var obj in item.objects)
        {
            go = Instantiate(obj.prefab, m_Character.GetBoneTransform(obj.targetBone));
            go.layer = m_Character.gameObject.layer;
            foreach (Transform child in go.transform)
                child.gameObject.layer = m_Character.gameObject.layer;
            equip.instantiatedObjects.Add(go);
        }

        //update bodyparts
        UpdateBodyRenderers();

        //map renderers
        InitRenderersForItem(equip);

        if (materialDatas != null)
            SetCustomizationMaterials(cat, equip.renderers, materialDatas);

        //send message to the character
        //used to update the facial blendshape controller and colliders for hair
        m_Character.SendMessage("OnChangeEquip", new object[] { cat, equip.instantiatedObjects }, SendMessageOptions.DontRequireReceiver);
    }

    private void InitRenderersForItem(KHHCustomizationDemo.EquipedItem item)
    {
        List<Renderer> renderers = new List<Renderer>();
        List<MaterialPropertyBlock> props = new List<MaterialPropertyBlock>();

        //get all materials in the instantiated items
        foreach (var obj in item.instantiatedObjects)
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());

        item.renderers = renderers.ToArray();

        //update the material properties for the new item
        foreach (var renderer in item.renderers)
            SyncNewItemMaterials(renderer);
    }

    public void UpdateBodyRenderers()
    {
        List<BodyPartType> disabled = new List<BodyPartType>();

        //get all parts that are hidden by equips
        foreach (var equip in m_Equiped.Values)
        {
            if (equip.assetReference == null)
                continue;

            foreach (var part in equip.assetReference.bodyParts)
                if (!disabled.Contains(part))
                    disabled.Add(part);
        }

        //set active value of each part
        foreach (var part in m_BodyParts)
            part.Value.gameObject.SetActive(!disabled.Contains(part.Key));

        //todo: maybe move this offset to the CharacterController's center offset or skin width
        var localPos = m_Character.transform.localPosition;
        localPos.y = m_Equiped.ContainsKey("shoes") ? 0.02f : 0;
        m_Character.transform.localPosition = localPos;
    }

    private void SyncNewItemMaterials(Renderer renderer)
    {
        //update the new renderers material with the stored properties
        for (int i = 0; i < renderer.sharedMaterials.Length; i++)
        {
            if (m_MaterialProperties.ContainsKey(renderer.sharedMaterials[i]) == false)
                continue;
            renderer.SetPropertyBlock(m_MaterialProperties[renderer.sharedMaterials[i]], i);
        }
    }

    public void SetCustomizationMaterials(string cat, Renderer[] renderers, List<KHHMaterialData> materialDatas = null)
    {
        if (cat == "body") return;

        if (renderers == null)
            renderers = new Renderer[0];

        //get all available materials
        List<Material> materials = new List<Material>();
        List<Renderer> renderersPerMaterial = new List<Renderer>();
        List<int> rendererMaterialIndex = new List<int>();
        List<MaterialPropertyBlock> propertyBlock = new List<MaterialPropertyBlock>();

        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            var sharedMaterials = renderer.sharedMaterials;

            for (int j = 0; j < sharedMaterials.Length; j++)
            {
                if (sharedMaterials[j].name.Contains("mouth"))
                    continue;
                if (materials.Contains(sharedMaterials[j]))
                    continue;
                materials.Add(sharedMaterials[j]);
                renderersPerMaterial.Add(renderer);
                rendererMaterialIndex.Add(j);

                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block, j);
                propertyBlock.Add(block);
            }
        }

        for (int i = 0; i < materials.Count; i++)
        {
            var auxRenderer = renderersPerMaterial[i];
            var auxMatIndex = rendererMaterialIndex[i];
            var auxPropertyBlock = propertyBlock[i];

            KHHMaterialData materialData = materialDatas == null ? null : materialDatas.Find(x => x.name == materials[i].name);

            if (materials[i].HasProperty("_MaskRemap"))
            {
                Color colorA = materials[i].GetColor("_Color_A_2");
                Color colorB = materials[i].GetColor("_Color_B_2");
                Color colorC = materials[i].GetColor("_Color_C_2");
                if (materialData != null)
                {
                    //if (materialData.ColorA != Color.black)
                    colorA = materialData.ColorA;
                    //if (materialData.ColorB != Color.black)
                    colorB = materialData.ColorB;
                    //if (materialData.ColorC != Color.black)
                    colorC = materialData.ColorC;
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_A_2", colorA);
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_B_2", colorB);
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_C_2", colorC);
                }
            }
            else if (materials[i].HasProperty("_BaseColor"))
            {
                Color color = materials[i].GetColor("_BaseColor");
                if (materialData != null)
                {
                    if (materialData.ColorA != Color.black)
                        color = materialData.ColorA;
                    OnChangeColor(auxRenderer, auxMatIndex, "_BaseColor", color);
                }
            }
        }
    }

    string GetAssetPath(string bodyType, string asset)
    {
        return $"{bodyType}/{asset}".ToLower();
    }

    private void OnChangeColor(Renderer renderer, int materialIndex, string property, Color color)
    {
        //update the renderer material property
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block, materialIndex);

        block.SetColor(property, color);
        renderer.SetPropertyBlock(block, materialIndex);

        //store the properties for this material in case a new equip needs it
        var sharedMaterial = renderer.sharedMaterials[materialIndex];
        m_MaterialProperties[sharedMaterial] = block;

        //update the property block of each renderer sharing the same material
        SyncMaterialChange(sharedMaterial, block);
    }

    private void SyncMaterialChange(Material sharedMaterial, MaterialPropertyBlock newProperties)
    {
        //apply the new properties to all renderers sharing the same material
        foreach (var equip in m_Equiped.Values)
        {
            foreach (var renderer in equip.renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] != sharedMaterial)
                        continue;
                    renderer.SetPropertyBlock(newProperties, i);
                }
            }
        }
    }
}
