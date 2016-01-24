using System;
using System.Collections.Generic;

namespace Segregation
{
	public enum TokenType
	{
		W,
		B,
		E,
	}

	public class Token
	{
		protected List<Token> neighborhood = new List<Token>();
		public void AddToNeighborhood (Token t)
		{
			neighborhood.Add(t);
		}
		public void ClearnNeighborhood()
		{
			neighborhood.Clear();
		}
		public List<Token> NPeek()
		{
			return neighborhood;
		}
		public virtual bool IsSatisfied()
		{
			return true;
		}
		public virtual bool TestSatisfied(List<Token> test)
		{
			return true;
		}
		protected TokenType type;
		protected int xpos; //x position
		protected int ypos; //y position
		public TokenType Type
		{
			get { return type; }
			set { type = value; }
		}
		public int Xpos
		{
			get { return xpos; }
			set { xpos = value; }
		}
		public int Ypos
		{
			get { return ypos; }
			set { ypos = value; }
		}
	}
	public class Person : Token
	{
		public Person(TokenType t, int x, int y)
		{
			this.type = t;
			xpos = x;
			ypos = y;
		}
		public override bool IsSatisfied()
		{
			double n = 0; //numerator
			double m = 0; //denominator
			foreach (Token t in neighborhood)
			{
				if (t.Type == TokenType.E ) {;}
				else if (t.Type == this.type) {m += 1; n += 1;}
				else { m += 1; }
			}
			return (m == 0 || n/m >= .5);
		}
		public override bool TestSatisfied (List<Token> test)
		{
			double n = 0; //numerator
			double m = 0; //denominator
			foreach (Token t in test)
			{
				if (t.Type == TokenType.E ) {;}
				else if (t.Type == this.type) {m += 1; n += 1;}
				else { m += 1; }
			}
			return (m == 0 || n/m >= .5);
		}
	}
	public class Empty : Token
	{
		public Empty(int x, int y)
		{
			type = TokenType.E;
			xpos = x;
			ypos = y;
		}
	}
	public class Board
	{
		private Token[,] positions;
		private int xsize; // number of columns in the board
		private int ysize; // number of rows in the board
		private int nsize; // size of neighborhood
		private List<Token> taken = new List<Token>();
		private List<Token> free = new List<Token>();
		private int movements = 0;
		public int Movements
		{
			get { return movements; }
			set { movements = value; }
		}		
		public Board(int x, int y, int nsize)
		{
			Random r = new Random();
			double temp = 0;
			this.positions = new Token[x,y];
			this.xsize = x;
			this.ysize = y;
			this.nsize = nsize;
			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < y; j++)
				{
					temp = r.NextDouble();
					if (temp < (.4))
						positions[i,j] = new Person(TokenType.W, i, j);
					else if (temp < (.8))
						positions[i,j] = new Person(TokenType.B, i, j);
					else
						positions[i,j] = new Empty(i, j);
				}
			}
		}
		public void PrintBoard()
		{
			for (int i = 0; i < xsize; i++ )
			{
				for (int j = 0; j < ysize; j++ )
				{
					if (j != ysize - 1)
						Console.Write("{0} ", positions[i,j].Type);
					else
						Console.WriteLine(positions[i,j].Type);
				}
			}
		}
		public void Move(int x, int y)
		{
			if (!positions[x,y].IsSatisfied())
			{
				foreach (Token candidate in free)
				{
					if(positions[x,y].TestSatisfied(candidate.NPeek()))
					{
						taken.Remove(positions[x,y]);
						free.Add(positions[x,y]);
						positions[candidate.Xpos,candidate.Ypos] = new Person(positions[x,y].Type, candidate.Xpos, candidate.Ypos);
						taken.Add(candidate);
						free.Add(positions[x,y]);
						positions[x,y] = new Empty(x,y);
						movements += 1;
						break;
					}
				}
			}
		}
		public void SetTaken() //method to reset list of taken locations
		{
			foreach (Token t in positions)
			{
				if(t.Type == TokenType.W || t.Type == TokenType.B)
					taken.Add(t);
			}
		}
		public void SetFree() //method to set list of free spaces
		{
			foreach (Token t in positions)
			{
				if(t.Type == TokenType.E)
					free.Add(t);
			}
		}
		public void SetNeighbors() //method to reset neighbors
		{
			foreach (Token mod in positions)
			{
				mod.ClearnNeighborhood();
				foreach(Token test in positions)
					if (test != mod && Math.Abs(mod.Xpos - test.Xpos) + Math.Abs(mod.Ypos - test.Ypos) <= nsize)
						mod.AddToNeighborhood(test);
			}

		}
		public List<Token> SimulateNeighbors(int x, int y)
		{
			List<Token> temp = new List<Token>();
			foreach(Token test in positions)
			{
				if (test.Xpos != x && test.Ypos != y && Math.Abs(x - test.Xpos) + Math.Abs(y - test.Ypos) <= nsize)
					temp.Add(test);
			}
			return temp;
		}
		public void Segregate()
		{
			bool everyoneSatisfied = false;
			while (!everyoneSatisfied)
			{
				PrintBoard();
				Console.WriteLine();
				SetTaken();
				SetNeighbors();
				SetFree();
				everyoneSatisfied = true;
				for (int i = 0; i < xsize; i++)
				{
					for (int j = 0; j < ysize; j++)
					{
						Move(i,j);
					}
				}
				foreach(Token t in positions)
				{
					if (!t.IsSatisfied())
						everyoneSatisfied = false;
				}
			}
		}
	}
	public class Run
	{
		public static void Main()
		{
			Board b = new Board(15,15,3);
			Console.WriteLine();
			b.Segregate();
			b.PrintBoard();
			Console.WriteLine("Number of moves: {0}", b.Movements);
		}
	}
}