using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TraitLogger : MonoBehaviour
{
	[SerializeField]
	public bool enable = false; // set true to log

	private static (double, string)[][] currentTraitTotals = new (double, string)[Species.GetValues(typeof(Species)).Length][];
	private static double[][] previousAveragTraits = new double[Species.GetValues(typeof(Species)).Length][];
	private static double[][,] previousAverageFCMs = new double[Species.GetValues(typeof(Species)).Length][,];
	private static List<FCMHandler>[] recentFCMs = new List<FCMHandler>[Species.GetValues(typeof(Species)).Length];
	private static int[] nAnimals = new int[Species.GetValues(typeof(Species)).Length];
	// current total might not exist when all animals are dead, but we still want to log so we need this
	private static int[] loggableSpecies = new int[Species.GetValues(typeof(Species)).Length];
	private static int[] bornAnimals = new int[Species.GetValues(typeof(Species)).Length];
	private static int deaths = 0;
	private static int ageDeaths = 0;
	private static int logInterval = 5; // seconds
	private static bool firstSave = true;
	private static string folder;
	//private static string fcmFolder;
	private static int round = 0;

	private static Hashtable animals = new Hashtable();
	private static Timer timer;
	private static bool logFirst = true;

	// Start is called before the first frame update
	void Start()
	{
		timer = new Timer(logInterval);
		timer.Start();
		if (enable)
		{
			folder = "Python Scripts and Logs/Logs/" + DateTime.Now.ToString("M-dd--HH-mm-ss");
			//fcmFolder = folder + "/fcms";
			Directory.CreateDirectory(folder);
			//Directory.CreateDirectory(fcmFolder);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!enable)
		{
			return;
		}
		if (logFirst || timer.IsDone())
		{
			for (int i = 0; i < Species.GetValues(typeof(Species)).Length; i++)
			{
				nAnimals[i] = 0; // refresh 
			}
			LogAndSave();
			timer.Reset();
			timer.Start();
			logFirst = false;
		}
	}

	void OnApplicationQuit()
	{
		if (enable)
		{
			WriteFCMsToFile();
		}
	}

	// Generate an average fcm of a species
	public static double[,] GenerateAverageFCM(Species s)
	{
		int nFields = Enum.GetNames(typeof(EntityField)).Length;
		double[,] averageWeights = new double[nFields, nFields];
		foreach (FCMHandler fcm in recentFCMs[(int)s])
		{
			double[,] weights = fcm.GetWeights();
			for (int _from = 0; _from < weights.GetLength(0); _from++)
			{
				for (int _to = 0; _to < weights.GetLength(1); _to++)
				{
					int denominator = nAnimals[(int)s];
					if (denominator == 0)
					{
						return previousAverageFCMs[(int)s];
					}
					else
					{
						averageWeights[_from, _to] += weights[_from, _to] / denominator;
					}

				}
			}
		}
		previousAverageFCMs[(int)s] = averageWeights;
		return averageWeights;
	}

	public static void SaveFCM(string content, string path)
	{
		string name = path + ".txt";
#if UNITY_EDITOR
		//name = path + ".txt";
#endif
#if UNITY_STANDALONE
		// You cannot add a subfolder, at least it does not work for me
		//name = path + ".txt";
#endif

		string str = content;
		using (FileStream fs = new FileStream(name, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(fs))
			{
				writer.Write(str);
			}
		}
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}

	public static void Log(AnimalTraits traits)
	{
		nAnimals[(int)traits.species]++;
		loggableSpecies[(int)traits.species] = 1;
		(double, string)[] traitValues = traits.GetNumericalTraits();

		if (nAnimals[(int)traits.species] == 1)
		{
			// refresh list
			currentTraitTotals[(int)traits.species] = traitValues;
			recentFCMs[(int)traits.species] = new List<FCMHandler>() { traits.fcmHandler };
		}
		else
		{
			for (int i = 0; i < traitValues.Length; i++)
			{
				currentTraitTotals[(int)traits.species][i].Item1 += traitValues[i].Item1;
			}
			recentFCMs[(int)traits.species].Add(traits.fcmHandler);
		}

	}

	// draw and or log current averages;
	private static void SaveTraitAndFCMGraphData()
	{
		StringBuilder traitRow = new StringBuilder("");
		StringBuilder[] fcmRows = new StringBuilder[Species.GetValues(typeof(Species)).Length];
		for (int i = 0; i < fcmRows.Length; i++)
			fcmRows[i] = new StringBuilder("");

		// save traits
		if (firstSave)
		{
			// instantiate previous averages with zeros
			for (int i = 0; i < loggableSpecies.Length; i++)
			{
				if (loggableSpecies[i] == 1)
				{
					previousAveragTraits[i] = new double[currentTraitTotals[i].Length];

					int size = Enum.GetNames(typeof(EntityField)).Length;
					previousAverageFCMs[i] = new double[size,size];
					fcmRows[i] = FCMHandler.ToCsvRow(GenerateAverageFCM((Species)i), true);
					fcmRows[i].Append('\n');
				}
			}
			// make header
			traitRow = MakeRow(true).Append("\n");
			firstSave = false;
		}
		traitRow.Append(MakeRow(false));

		for (int i = 0; i < fcmRows.Length; i++)
		{
			if (loggableSpecies[i] == 1)
				fcmRows[i].Append(FCMHandler.ToCsvRow(GenerateAverageFCM((Species)i), false));
		}

		//File.WriteAllText(filename, row.ToString());
		using (StreamWriter writeText = new StreamWriter(folder + '/' + "Traits" + ".txt", true))
		{
			writeText.WriteLine(traitRow.ToString());
		}

		for (int i = 0; i < fcmRows.Length; i++)
		{
			if (loggableSpecies[i] == 1)
			{
				using (StreamWriter writeText = new StreamWriter(folder + '/' + ((Species)i) + "_fcmGraph.txt", true))
				{
					writeText.WriteLine(fcmRows[i].ToString());
				}
			}
		}
	}

	private static StringBuilder MakeRow(bool isHeader)
	{
		StringBuilder row = new StringBuilder("");
		for (int i = 0; i < loggableSpecies.Length; i++)
		{
			// make 1 column or each trait and species
			if (loggableSpecies[i] == 1)
			{
				// make entry for each population and born children
				if (isHeader)
				{
					row.Append(((Species)i).ToString());
					row.Append('-');
					row.Append("population,");

					row.Append(((Species)i).ToString());
					row.Append('-');
					row.Append("born children,");
				}
				else
				{
					row.Append(nAnimals[i]);
					row.Append(",");

					row.Append(bornAnimals[i]);
					row.Append(",");

				}
				// make an entry for each trait
				for (int j = 0; j < currentTraitTotals[i].Length; j++)
				{
					if (isHeader)
					{
						// "e.g Rabbit-speed"
						row.Append(((Species)i).ToString());
						row.Append('-');
						row.Append(currentTraitTotals[i][j].Item2);
					}
					else
					{
						// average
						if (nAnimals[i] != 0)
						{
							double average = currentTraitTotals[i][j].Item1 / nAnimals[i];
							previousAveragTraits[i][j] = average;
						}
						row.Append(previousAveragTraits[i][j].ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
					}
					row.Append(",");
				}
			}
		}
		if (row.Length > 0)
		{
			row.Length -= 1; // remove ","
		}
		return row;
	}

	public static void Register(Animal a)
	{
		animals.Add(a.GetHashCode(), a);
	}

	public static void Unregister(Animal a, CauseOfDeath cause)
	{
		animals.Remove(a.GetHashCode());
		deaths++;
		if (cause == CauseOfDeath.Age)
			ageDeaths++;
	}

	public static void LogAndSave()
	{
		foreach (DictionaryEntry e in animals)
		{
			Log(((Animal)e.Value).GetTraits());
		}
		SaveTraitAndFCMGraphData();
	}

	public static void StartNewRound()
	{
		timer.Reset();
		timer.Start();
		logFirst = true;
		bornAnimals = new int[Species.GetValues(typeof(Species)).Length];
		WriteFCMsToFile();
		round++;
		deaths = 0;
		ageDeaths = 0;
	}

	public static void RegisterBirth(Species s)
	{
		bornAnimals[(int)s]++;
	}

	private static void WriteFCMsToFile()
	{
		// save fcm
		for (int i = 0; i < recentFCMs.Length; i++)
		{
			if (recentFCMs[i] != null)
			{
				StringBuilder s = FCMHandler.ToCsv(GenerateAverageFCM((Species)i));
				SaveFCM(s.ToString(), folder  + "/round " + round + " " + ((Species)i).ToString() + "_fcm");
			}
		}
	}

	public static void LogRound(int fitness, int time, int maxCreatures)
	{
		string logRow = "";

		if (round != 0)
		{
			// normal row
			string deathRatio = "-1";
			if (deaths != 0)
			{
				deathRatio = (ageDeaths / (double)deaths).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture);
			}
			int bornAnimals = 0;
			for (int i = 0; i < TraitLogger.bornAnimals.Length; i++)
			{
				bornAnimals += TraitLogger.bornAnimals[i];
			}
			logRow = fitness + "," + time + "," + maxCreatures + "," + deathRatio + "," + bornAnimals;
		}
		else
		{
			// header row
			logRow = "Fitness,Round duration,Maximum alive animals,Age death ratio,Born animals";
		}

		using (StreamWriter writeText = new StreamWriter(folder + '/' + "RoundSummary" + ".txt", true))
		{
			writeText.WriteLine(logRow);
		}
	}
}
