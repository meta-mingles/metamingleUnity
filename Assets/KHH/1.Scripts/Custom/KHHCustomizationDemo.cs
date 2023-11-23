using Rukha93.ModularAnimeCharacter.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHCustomizationDemo : MonoBehaviour
{
    public static readonly List<string> CustomCategories = new List<string>
        {
            "body",
            "head",
            "hairstyle",
            "top",
            "bottom",
            "shoes",
            "outfit"
        };

    public class EquipedItem
    {
        public string path;
        public List<GameObject> instantiatedObjects;
        public CustomizationItemAsset assetReference;

        //for material customization
        public Renderer[] renderers;
    }

    [SerializeField] private KHHUICustomizationDemo m_UI;

    private IAssetLoader m_AssetLoader;

    private Animator m_Character;
    private SkinnedMeshRenderer m_ReferenceMesh;

    private Dictionary<string, List<string>> m_CustomizationOptions; //<categoryId, assetPath[]>

    private Dictionary<BodyPartType, BodyPartTag> m_BodyParts;
    private Dictionary<string, EquipedItem> m_Equiped;
    private Dictionary<Material, MaterialPropertyBlock> m_MaterialProperties;

    private Coroutine m_LoadingCoroutine;

    private void Awake()
    {
        //init variables
        m_AssetLoader = GetComponentInChildren<IAssetLoader>();
        m_CustomizationOptions = new Dictionary<string, List<string>>();
        m_Equiped = new Dictionary<string, EquipedItem>();
        m_BodyParts = new Dictionary<BodyPartType, BodyPartTag>();
        m_MaterialProperties = new Dictionary<Material, MaterialPropertyBlock>();

        //ui callbacks
        m_UI.OnClickCategory += OnSelectCategory;
        m_UI.OnChangeItem += OnSwapItem;
        m_UI.OnChangeColor += OnChangeColor;
    }

    async void Start()
    {
        //init categories UI
        m_UI.SetCategories(CustomCategories.ToArray());
        for (int i = 0; i < CustomCategories.Count; i++)
            m_UI.SetCategoryValue(i, "");

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
    }

    private void InitBody(string path, GameObject prefab)
    {
        //instantiate the body prefab and store the animator
        m_Character = Instantiate(prefab, this.transform).GetComponent<Animator>();

        //get a random body mesh to be used as reference
        var meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        m_ReferenceMesh = meshes[meshes.Length / 2];

        //initialize all tagged body parts
        //they be used to disable meshes that are hidden by clothes
        var bodyparts = m_Character.GetComponentsInChildren<BodyPartTag>();
        foreach (var part in bodyparts)
            m_BodyParts[part.type] = part;

        var equip = new EquipedItem()
        {
            path = path,
            assetReference = null,
            instantiatedObjects = new List<GameObject>() { m_Character.gameObject }
        };
        InitRenderersForItem(equip);
        m_Equiped["body"] = equip;

        //update ui
        m_UI.SetCategoryValue(CustomCategories.IndexOf("body"), path);
        if (m_UI.IsCustomizationOpen && m_UI.CurrentCategory == "body")
            m_UI.SetCustomizationMaterials(equip.renderers);
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
            KHHUserCustom.Clear();
        }

        //init the customization options for the selected body type
        List<Coroutine> coroutines = new List<Coroutine>();
        for (int i = 0; i < CustomCategories.Count; i++)
        {
            int index = i;
            string path = CustomCategories[i].Equals("body") ? "body" : GetAssetPath(bodyType, CustomCategories[i]);
            coroutines.Add(StartCoroutine(m_AssetLoader.LoadAssetList(path, res => m_CustomizationOptions[CustomCategories[index]] = new List<string>(res))));
        }
        for (int i = 0; i < CustomCategories.Count; i++)
        {
            yield return coroutines[i];

            //add an empty item for all categories that can be empty
            if (CustomCategories[i].Equals("body"))
                continue;
            if (CustomCategories[i].Equals("head"))
                continue;
            m_CustomizationOptions[CustomCategories[i]].Insert(0, "");
        }

        //initialize the body
        var bodyPath = GetAssetPath(bodyType, "body");
        yield return m_AssetLoader.LoadAsset<GameObject>(bodyPath, res => InitBody(bodyPath, res));

        //initialize the head with the first available
        string assetPath = m_CustomizationOptions["head"][0];
        yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(assetPath, res => Equip("head", assetPath, res));

        m_LoadingCoroutine = null;
    }

    #region EQUIPMENT

    public void Equip(string cat, string path, CustomizationItemAsset item, List<KHHMaterialData> materialDatas = null)
    {
        if (string.IsNullOrEmpty(path) || item == null) return;

        //if outfit, remove all othet pieces
        if (cat.Equals("outfit"))
        {
            Unequip("top", false);
            Unequip("bottom", false);
        }
        else if (cat.Equals("top") || cat.Equals("bottom"))
        {
            Unequip("outfit", false);
        }

        //unequip previous item
        Unequip(cat, false);

        EquipedItem equip = new EquipedItem()
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
            equip.instantiatedObjects.Add(go);
        }

        //update bodyparts
        UpdateBodyRenderers();

        //map renderers
        InitRenderersForItem(equip);

        //update ui
        m_UI.SetCategoryValue(CustomCategories.IndexOf(cat), path);
        if ((m_UI.IsCustomizationOpen && m_UI.CurrentCategory == cat) || materialDatas != null)
            m_UI.SetCustomizationMaterials(equip.renderers, materialDatas);

        //send message to the character
        //used to update the facial blendshape controller and colliders for hair
        m_Character.SendMessage("OnChangeEquip", new object[] { cat, equip.instantiatedObjects }, SendMessageOptions.DontRequireReceiver);
    }

    //public void LoadColor(Renderer[] renderers, List<KHHMaterialData> materialDatas)
    //{
    //    //get all available materials
    //    List<Material> materials = new List<Material>();
    //    List<Renderer> renderersPerMaterial = new List<Renderer>();
    //    List<int> rendererMaterialIndex = new List<int>();
    //    List<MaterialPropertyBlock> propertyBlock = new List<MaterialPropertyBlock>();

    //    for (int i = 0; i < renderers.Length; i++)
    //    {
    //        var renderer = renderers[i];
    //        var sharedMaterials = renderer.sharedMaterials;

    //        for (int j = 0; j < sharedMaterials.Length; j++)
    //        {
    //            if (sharedMaterials[j].name.Contains("mouth"))
    //                continue;
    //            if (materials.Contains(sharedMaterials[j]))
    //                continue;
    //            materials.Add(sharedMaterials[j]);
    //            renderersPerMaterial.Add(renderer);
    //            rendererMaterialIndex.Add(j);

    //            var block = new MaterialPropertyBlock();
    //            renderer.GetPropertyBlock(block, j);
    //            propertyBlock.Add(block);
    //        }
    //    }

    //    for (int i = 0; i < materials.Count; i++)
    //    {
    //        KHHMaterialData materialData = materialDatas.Find(x => x.name == materials[i].name);
    //        if (materialData == null) continue;

    //        var auxRenderer = renderersPerMaterial[i];
    //        var auxMatIndex = rendererMaterialIndex[i];
    //        var auxPropertyBlock = propertyBlock[i];

    //        if (materials[i].HasProperty("_MaskRemap"))
    //        {
    //            //customization channel 1
    //            Color colorA = materialData.ColorA;
    //            if (colorA.r != 0 || colorA.g != 0 || colorA.b != 0)
    //                OnChangeColor(auxRenderer, auxMatIndex, "_Color_A_1", colorA);
    //            //customization channel 2
    //            Color colorB = materialData.ColorB;
    //            if (colorB.r != 0 || colorB.g != 0 || colorB.b != 0)
    //                OnChangeColor(auxRenderer, auxMatIndex, "_Color_B_1", colorB);
    //            //customization channel 3
    //            Color colorC = materialData.ColorC;
    //            if (colorC.r != 0 || colorC.g != 0 || colorC.b != 0)
    //                OnChangeColor(auxRenderer, auxMatIndex, "_Color_C_1", colorC);

    //        }
    //        else if (materials[i].HasProperty("_BaseColor"))
    //        {
    //            Color color = materialData.ColorA;
    //            if (color.r != 0 || color.g != 0 || color.b != 0)
    //                OnChangeColor(auxRenderer, auxMatIndex, "_BaseColor", color);
    //        }
    //    }
    //}

    private IEnumerator Co_LoadAndEquip(string cat, string path, List<KHHMaterialData> materialDatas = null)
    {
        yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(path, res => Equip(cat, path, res, materialDatas));
        m_LoadingCoroutine = null;
    }

    public void Unequip(string category, bool updateRenderers = true)
    {
        if (m_Equiped.ContainsKey(category) == false)
            return;

        var item = m_Equiped[category];
        m_Equiped.Remove(category);

        //destroy instances
        foreach (var go in item.instantiatedObjects)
            Destroy(go);

        //update body parts
        if (updateRenderers)
            UpdateBodyRenderers();

        //update UI
        m_UI.SetCategoryValue(CustomCategories.IndexOf(category), "");
        if (m_UI.IsCustomizationOpen)
            m_UI.SetCustomizationMaterials(null);
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

    #endregion
    private void InitRenderersForItem(EquipedItem item)
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

    #region HELPERS

    public string GetAssetPath(string bodyType, string asset)
    {
        return $"{bodyType}/{asset}".ToLower();
    }

    public string GetAssetPath(string bodyType, string category, string asset)
    {
        return $"{bodyType}/{category}/{asset}".ToLower();
    }

    #endregion

    #region UI CALLBACKS

    private void OnSelectCategory(string cat)
    {
        if (string.Equals(m_UI.CurrentCategory, cat))
        {
            m_UI.ShowCustomization(false);
            return;
        }

        //init items
        m_UI.SetCustomizationOptions(cat, m_CustomizationOptions[cat].ToArray(), m_Equiped.ContainsKey(cat) ? m_Equiped[cat].path : "");

        //set material options
        if (m_Equiped.ContainsKey(cat))
            m_UI.SetCustomizationMaterials(m_Equiped[cat].renderers);
        else
            m_UI.SetCustomizationMaterials(null);

        //show UI
        m_UI.ShowCustomization(true);

        //착용 물건 변경에 따른 저장
        KHHUserCustom.SetCategory(cat);
    }

    private void OnSwapItem(string cat, string asset)
    {
        //if empty, just unequip the current one if any
        if (string.IsNullOrEmpty(asset))
        {
            Unequip(cat);
            return;
        }

        //stop loading previous item
        if (m_LoadingCoroutine != null)
            StopCoroutine(m_LoadingCoroutine);

        //load new item
        if (cat.Equals("body"))
            m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody(asset.StartsWith("m") ? "m" : "f"));
        else
            m_LoadingCoroutine = StartCoroutine(Co_LoadAndEquip(cat, asset));

        //착용 물건 변경에 따른 저장
        KHHUserCustom.SetItem(m_CustomizationOptions[cat].IndexOf(asset));
    }

    private void OnChangeColor(Renderer renderer, int materialIndex, string property, Color color)
    {
        //update the renderer material property
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block, materialIndex);

        block.SetColor(property, color);
        renderer.SetPropertyBlock(block, materialIndex);

        if (renderer.name.Contains("head.001_mesh") && materialIndex == 0)
        {
            MaterialPropertyBlock block2 = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block2, 1);
            block2.SetColor(property, color);
            renderer.SetPropertyBlock(block2, 1);
        }

        //store the properties for this material in case a new equip needs it
        var sharedMaterial = renderer.sharedMaterials[materialIndex];
        m_MaterialProperties[sharedMaterial] = block;

        //update the property block of each renderer sharing the same material
        SyncMaterialChange(sharedMaterial, block);
    }

    #endregion
}