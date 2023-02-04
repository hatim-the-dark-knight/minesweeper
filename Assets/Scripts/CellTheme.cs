using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprites", menuName = "Cells")]
public class CellTheme : ScriptableObject
{
	public CellSprites cellSprites;

	public Sprite GetCellSprite(int cell)
	{
		switch (cell)
		{
			case -1:
				return cellSprites.detonatedMine;
			case 0:
				return cellSprites.empty;
			case 1:
				return cellSprites.one;
			case 2:
				return cellSprites.two;
			case 3:
				return cellSprites.three;
			case 4:
				return cellSprites.four;
			case 5:
				return cellSprites.five;
			case 6:
				return cellSprites.six;
			case 7:
				return cellSprites.seven;
			case 8:
				return cellSprites.eight;
			case 9:
				return cellSprites.flag;
			case 10:
				return cellSprites.mine;
			case 11:
				return cellSprites.incorrectFlag;
			case 12:
				return cellSprites.cell;
			default:
				Debug.Log(cell);
				return null;
		}
	}

	[System.Serializable]
	public class CellSprites
	{
		public Sprite cell, empty, one, two, three, four, five, six, seven, eight, mine, detonatedMine, flag, incorrectFlag;

		public Sprite this[int i]
		{
			get
			{
				return new Sprite[] { cell, empty, one, two, three, four, five, six, seven, eight, mine, detonatedMine, flag, incorrectFlag }[i];
			}
		}
	}
}
