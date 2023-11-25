using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KHHUICustomizationDemo : MonoBehaviour
{
    [SerializeField] private Button m_SaveButton;
    [Header("category")]
    [SerializeField] private LayoutGroup m_CategoryPanel;
    [SerializeField] private KHHUICategoryItem m_CategoryItemPrefab;

    [Header("customization")]
    [SerializeField] private Text m_CustomizationTitle;
    [SerializeField] private LayoutGroup m_CustomizationPanel;
    [SerializeField] private KHHUISwapperItem m_SwapperItem;
    [SerializeField] private KHHUIMaterialItem m_MaterialItemPrefab;

    [Space]
    [SerializeField] private KHHUIColorPicker m_ColorPicker;

    public string CurrentCategory { get; private set; }

    private List<KHHUICategoryItem> m_CategoryItems = new List<KHHUICategoryItem>();
    private List<KHHUIMaterialItem> m_MaterialItems = new List<KHHUIMaterialItem>();

    private List<string> m_ItemOptions;
    private int m_CurrentItemIndex;

    public System.Action<string> OnClickCategory;
    public System.Action<string, string> OnChangeItem;
    public System.Action<Renderer, int, string, Color> OnChangeColor;

    public bool IsCustomizationOpen => m_CustomizationPanel.gameObject.activeSelf;


    private void Awake()
    {
        m_CategoryItemPrefab.gameObject.SetActive(false);
        m_MaterialItemPrefab.gameObject.SetActive(false);

        m_SwapperItem.OnClickLeft = _OnSwapPrevious;
        m_SwapperItem.OnClickRight = _OnSwapNext;

        if (m_SaveButton != null)
            m_SaveButton.onClick.AddListener(SaveButtonEvent);
    }

    private void Start()
    {
        ShowCustomization(false);
        m_ColorPicker.Close();
    }

    #region CATEGORY PANEL

    public void SetCategories(string[] categories)
    {
        for (int i = 0; i < categories.Length; i++)
        {
            KHHUICategoryItem item;
            if (i < m_CategoryItems.Count)
            {
                item = m_CategoryItems[i];
            }
            else
            {
                item = Instantiate(m_CategoryItemPrefab, m_CategoryPanel.transform);
                m_CategoryItems.Add(item);
            }

            item.gameObject.SetActive(true);
            item.Title = categories[i];

            string aux = categories[i];
            item.OnClick = () =>
            {
                foreach (var categoryItem in m_CategoryItems)
                    categoryItem.SetOff();
                OnClickCategory?.Invoke(aux);
            };
        }

        for (int i = categories.Length; i < m_CategoryItems.Count; i++)
            m_CategoryItems[i].gameObject.SetActive(false);
    }

    public void SetCategoryValue(int index, string value)
    {
        KHHUICategoryItem item = m_CategoryItems[index];
        item.Value = value;
    }

    #endregion

    #region CUSTOMIZATION PANEL

    public void ShowCustomization(bool value)
    {
        //todo: add basic animation

        if (value == false)
        {
            CurrentCategory = "";
            m_CustomizationPanel.gameObject.SetActive(false);
            m_ColorPicker.Close();
            return;
        }

        m_CustomizationPanel.gameObject.SetActive(true);
    }

    public void SetCustomizationOptions(string category, string[] items, string currentItem)
    {
        CurrentCategory = category;
        m_ItemOptions = new List<string>(items);
        m_CurrentItemIndex = Mathf.Max(m_ItemOptions.IndexOf(currentItem), 0);

        m_CustomizationTitle.text = category;

        //item swapping
        if (m_CurrentItemIndex >= m_ItemOptions.Count)
        {
            m_SwapperItem.gameObject.SetActive(false);
            return;
        }

        m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];
        m_SwapperItem.gameObject.SetActive(true);
    }

    public void SetCustomizationMaterials(Renderer[] renderers, List<KHHMaterialData> materialDatas = null)
    {
        if (CurrentCategory == "body")
        {
            for (int i = 0; i < m_MaterialItems.Count; i++)
                m_MaterialItems[i].gameObject.SetActive(false);
            m_ColorPicker.Close();
            return;
        }

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
                if (sharedMaterials[j].name.Contains("face"))
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
            //item from pool or instantiate
            KHHUIMaterialItem item;
            if (i < m_MaterialItems.Count)
            {
                item = m_MaterialItems[i];
            }
            else
            {
                item = Instantiate(m_MaterialItemPrefab, m_CustomizationPanel.transform);
                item.ColorPicker = m_ColorPicker;
                m_MaterialItems.Add(item);
            }
            item.gameObject.SetActive(true);

            //setup item
            item.Title = materials[i].name;

            var auxRenderer = renderersPerMaterial[i];
            var auxMatIndex = rendererMaterialIndex[i];
            var auxPropertyBlock = propertyBlock[i];

            KHHMaterialData materialData = materialDatas == null ? null : materialDatas.Find(x => x.name == materials[i].name);

            if (materials[i].HasProperty("_MaskRemap"))
            {
                item.ResetColors();

                Color colorA, colorB, colorC, newColorA, newColorB, newColorC;
                colorA = newColorA = materials[i].GetColor("_Color_A_2");
                colorB = newColorB = materials[i].GetColor("_Color_B_2");
                colorC = newColorC = materials[i].GetColor("_Color_C_2");
                if (materialData != null)
                {
                    newColorA = materialData.ColorA;
                    newColorB = materialData.ColorB;
                    newColorC = materialData.ColorC;
                    OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_2", newColorA);
                    OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_2", newColorB);
                    OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_2", newColorC);
                }

                //customization channel 1
                if (colorA.r != 0 || colorA.g != 0 || colorA.b != 0)
                    item.SetColor(auxPropertyBlock.HasColor("_Color_A_2") ? auxPropertyBlock.GetColor("_Color_A_2") : newColorA, 0,
                        (c) => OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_2", c));
                //customization channel 2
                if (colorB.r != 0 || colorB.g != 0 || colorB.b != 0)
                    item.SetColor(auxPropertyBlock.HasColor("_Color_B_2") ? auxPropertyBlock.GetColor("_Color_B_2") : newColorB, 1,
                        (c) => OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_2", c));
                //customization channel 3
                if (colorC.r != 0 || colorC.g != 0 || colorC.b != 0)
                    item.SetColor(auxPropertyBlock.HasColor("_Color_C_2") ? auxPropertyBlock.GetColor("_Color_C_2") : newColorC, 2,
                        (c) => OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_2", c));
            }
            else if (materials[i].HasProperty("_BaseColor"))
            {
                item.ResetColors();

                Color color = materials[i].GetColor("_BaseColor");
                if (materialData != null)
                {
                    if (materialData.ColorA != Color.black)
                        color = materialData.ColorA;
                    OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_BaseColor", color);
                }
                item.SetColor(auxPropertyBlock.HasColor("_BaseColor") ? auxPropertyBlock.GetColor("_BaseColor") : color, 0,
                    (c) => OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_BaseColor", c));
            }
        }

        for (int i = materials.Count; i < m_MaterialItems.Count; i++)
            m_MaterialItems[i].gameObject.SetActive(false);
        m_ColorPicker.Close();
    }

    private void _OnSwapPrevious()
    {
        m_CurrentItemIndex = m_CurrentItemIndex - 1;
        if (m_CurrentItemIndex < 0)
            m_CurrentItemIndex = m_ItemOptions.Count - 1;
        m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];

        OnChangeItem?.Invoke(CurrentCategory, m_ItemOptions[m_CurrentItemIndex]);
    }

    private void _OnSwapNext()
    {
        m_CurrentItemIndex = (m_CurrentItemIndex + 1) % m_ItemOptions.Count;
        m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];

        OnChangeItem?.Invoke(CurrentCategory, m_ItemOptions[m_CurrentItemIndex]);
    }

    #endregion

    void SaveButtonEvent()
    {
        KHHUserCustom.SaveData(() =>
        {
            string newSceneName = GlobalValue.PrevSceneName;
            if (newSceneName.Contains("Tool"))
            {
                GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
                GlobalValue.CurSceneName = newSceneName;
                SceneManager.LoadScene(newSceneName);
            }
            else
            {
                GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
                GlobalValue.CurSceneName = newSceneName;
                KHHPhotonInit.instance.ReJoinRoom(SceneManager.GetActiveScene().name, newSceneName);
                //PhotonNetwork.LoadLevel(newSceneName);
            }
        });
    }
}