using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    private ItemDetailList_SO dataBase;
    private List<ItemDetails> itemlist = new List<ItemDetails>();
    private VisualTreeAsset itemTemplate;
    private ListView itemListView;
    private ScrollView itemDetailSection;
    private ItemDetails activeItem;
    private VisualElement iconPreview;
    private Sprite defaultIcon;

    #region 默认写法，创建生成
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    // 在顶部创建快捷方式
    [MenuItem("ItemList/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }
    #endregion

    /// <summary>
    /// 主函数
    /// </summary>
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        // 获取模板文件的路径
        itemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRow Template.uxml");
        // 获取左侧item列表的元素位置
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailSection = root.Q<ScrollView>("ItemView");
        iconPreview = itemDetailSection.Q<VisualElement>("Icon");
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_Game.png");

        root.Q<Button>("AddButton").clicked += AddItem;
        root.Q<Button>("DeleteButton").clicked += DeleteItem;

        LoadDataBase();

        GenerateListView();
    }


    void test()
    {
        itemDetailSection.visible = false;
    }
    private void AddItem()
    {
       ItemDetails newItem = new ItemDetails();
       itemlist.Add(newItem);
       newItem.itemName = "NEW ITEM";
       newItem.itemID = 1000 + itemlist.Count;
       itemListView.Rebuild();

    }

    private void DeleteItem()
    {
        itemDetailSection.visible = false;
        itemlist.Remove(activeItem);
        itemListView.Rebuild();
    }


    /// <summary>
    /// 查找 ItemDetailList_SO 文件位置，把其中的列表数据拉取到当前临时列表中，方便调用
    /// 主要应用的类名为 AssetDataBase
    /// </summary>
    private void LoadDataBase()
    {
        // 寻找文件夹中的 ItemDetailList_SO 文件，返回其中的GUIDs
        // 解释：GUIDs是unity给资源分配的唯一标识
        // 这里是查询这个文件的“身份证号”，返回一个String的数组
        var dataArry = AssetDatabase.FindAssets("ItemDetailList_SO");

        if (dataArry.Length > 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArry[0]); //返回文件所在的文件夹路径
            // 根据路径打开文件,一共有两种写法，第二种比较方便
            // dataBase = (ItemDetailList_SO)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDetailList_SO));
            dataBase = AssetDatabase.LoadAssetAtPath<ItemDetailList_SO>(path);
        }
        itemlist = dataBase.itemDetailsList;
        // ***如果不标记就没办法保存数据***
        EditorUtility.SetDirty(dataBase);
    }

    /// <summary>
    /// 在Editor中生成数据库中的列表
    /// </summary>
    void GenerateListView()
    {
        //创建一行,克隆一份模板
        Func<VisualElement> makeItem = () => itemTemplate.CloneTree();

        // 绑定数据方法,e是每个变量,i是变量的序号
        Action<VisualElement, int> bindItme = (e, i) =>
        {
            if (i < itemlist.Count)
            {
                if (itemlist[i].itemIcon != null)
                    // Q方法是查找方法, Q<查找的类型>("查找的Name")
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemlist[i].itemIcon.texture;
                e.Q<Label>("Name").text = itemlist[i] == null ? "NO NAME" : itemlist[i].itemName;
            }
        };

        itemListView.itemsSource = itemlist;     // list列表等于数据库列表
        itemListView.makeItem = makeItem;       // 默认写法
        itemListView.bindItem = bindItme;      // 默认写法

        itemListView.selectionChanged += OnListSelectionChange;  // 默认事件，选中listView会发生什么
        itemDetailSection.visible = false;  // 右侧默认不显示
    }

    private void OnListSelectionChange(IEnumerable<object> secectItem)
    {
        activeItem = (ItemDetails)secectItem.First();
        GetItemDetails();
        itemDetailSection.visible = true;
    }

    void GetItemDetails()
    {
        // 设置标记，否则没有办法保存
        itemDetailSection.MarkDirtyRepaint();
        itemDetailSection.Q<IntegerField>("ItemID").value = activeItem.itemID;
        // 如果在editor中修改了数据，则会调用callback方法，同步数据库中的数据
        itemDetailSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evi => activeItem.itemID = evi.newValue);


        itemDetailSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evi =>
        {
            activeItem.itemName = evi.newValue;
            itemListView.Rebuild(); // 只有两个部分需要写重建
        });

        // 三元运算符保证新建时没有默认图片不会报错，而是直接给一个默认图片
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture;
        // 注意这里ObjectField的命名空间为UnityEditor.UIElements;
        // 很容易引用错误
        itemDetailSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evi =>
        {
            Sprite newIcon = (Sprite)evi.newValue;
            activeItem.itemIcon = newIcon;
            // 注意这里和上面一样，newIcon这里不需要精确到texture否则会出现错误
            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture;
            itemListView.Rebuild();
        });


        itemDetailSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evi => activeItem.itemType = (ItemType)evi.newValue);

        itemDetailSection.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        itemDetailSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evi => activeItem.itemOnWorldSprite = (Sprite)evi.newValue);

        itemDetailSection.Q<TextField>("ItemDescription").value = activeItem.itemDescription;
        itemDetailSection.Q<TextField>("ItemDescription").RegisterValueChangedCallback(evi => activeItem.itemDescription = evi.newValue);

        itemDetailSection.Q<IntegerField>("UseRadius").value = activeItem.itemUseRadius;
        itemDetailSection.Q<IntegerField>("UseRadius").RegisterValueChangedCallback(evi => activeItem.itemUseRadius = evi.newValue);

        itemDetailSection.Q<Toggle>("CanPickedup").value = activeItem.canPickedup;
        itemDetailSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evi => activeItem.canPickedup = evi.newValue);

        itemDetailSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evi => activeItem.canDropped = evi.newValue);


        itemDetailSection.Q<Toggle>("CanCarred").value = activeItem.canCarried;
        itemDetailSection.Q<Toggle>("CanCarred").RegisterValueChangedCallback(evi => activeItem.canCarried = evi.newValue);

        itemDetailSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evi => activeItem.itemPrice = evi.newValue);

        itemDetailSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evi => activeItem.sellPercentage = evi.newValue);

        
    }
}
