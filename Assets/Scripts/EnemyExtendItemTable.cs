using System.Globalization;
using System.Xml;
using UnityEngine;

public class EnemyExtendItemTable
{
	public const int ITEM_COUNT_MAX = 3;

	public const int TBL_COUNT_MAX = 1;

	private static readonly string[] ITEM_NAMES = new string[3]
	{
		"BronzeTimer",
		"SilverTimer",
		"GoldTimer"
	};

	private int[] m_tblInfo;

	private int m_tblCount;

	public void Setup(TerrainXmlData terrainData)
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new int[3];
		}
		if (!(terrainData != null))
		{
			return;
		}
		TextAsset enemyExtendItemTableData = terrainData.EnemyExtendItemTableData;
		if ((bool)enemyExtendItemTableData)
		{
			string xml = AESCrypt.Decrypt(enemyExtendItemTableData.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, m_tblInfo, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static string GetItemName(uint index)
	{
		if (index < 3 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public int GetTableItemData(EnemyExtendItemTableItem item)
	{
		return GetData((int)item);
	}

	public static void CreateTable(XmlDocument doc, int[] data, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EnemyExtendItemTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (XmlNode item in xmlNodeList)
		{
			XmlNodeList xmlNodeList2 = item.SelectNodes("Item");
			foreach (XmlNode item2 in xmlNodeList2)
			{
				for (int i = 0; i < 3; i++)
				{
					string itemName = GetItemName((uint)i);
					XmlAttribute xmlAttribute = item2.Attributes[itemName];
					int num2 = 0;
					if (xmlAttribute != null)
					{
						num2 = int.Parse(item2.Attributes[itemName].Value, NumberStyles.AllowLeadingSign);
					}
					int num3 = num * 3 + i;
					data[num3] = num2;
				}
			}
			num++;
		}
		tbl_count = num;
	}

	public bool IsSetupEnd()
	{
		if (m_tblInfo == null)
		{
			return false;
		}
		return true;
	}

	private int GetData(int tbl_index, int item_index)
	{
		if (m_tblInfo != null && (uint)tbl_index < m_tblCount)
		{
			int num = tbl_index * 3 + item_index;
			if (num < m_tblInfo.Length)
			{
				return m_tblInfo[num];
			}
		}
		return 0;
	}

	private int GetData(int index)
	{
		if (m_tblInfo != null && index < m_tblInfo.Length)
		{
			return m_tblInfo[index];
		}
		return 0;
	}
}