/*
 * Deterministic Roulette Game - Bet Table Generator
 * 
 * Author Berkan Özgür
 * 
 * Editor tool to automatically generate bet locations for inside bets.
 * 
 * !-   Warning: This script is designed for editor use only. Do not call GenerateAllBetLocations() at runtime.   -!
 * !-                        This script could delete/override your bet table setup.                              -!
 * 
 * This script cannot generate and place BetLocation precisely, it still needs editing.
 * Its main purpose is to save time by creating a base layout of bet locations that can be further adjusted and customized.
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class BetTableGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject betLocationPrefab;

    [Header("Table Layout")]
    [SerializeField] private Transform tableParent;
    [SerializeField] private float cellWidth = 1f;
    [SerializeField] private float cellHeight = 1f;
    [SerializeField] private Vector3 tableStartPosition = Vector3.zero;

    [Header("Bet Location Sizes")]
    [SerializeField] private Vector3 straightBetSize = new Vector3(0.8f, 0.1f, 0.8f);
    [SerializeField] private Vector3 splitBetSize = new Vector3(0.2f, 0.1f, 0.8f);
    [SerializeField] private Vector3 cornerBetSize = new Vector3(0.2f, 0.1f, 0.2f);
    [SerializeField] private Vector3 streetBetSize = new Vector3(0.8f, 0.1f, 0.2f);
    [SerializeField] private Vector3 sixLineBetSize = new Vector3(0.8f, 0.1f, 0.2f);

    [Header("Visual Settings")]
    [SerializeField] private string betLocationLayer = "BetLocation";

    [Header("Generation Options")]
    [SerializeField] public bool generateStraights = true;
    [SerializeField] public bool generateSplits = true;
    [SerializeField] public bool generateCorners = true;
    [SerializeField] public bool generateStreets = true;
    [SerializeField] public bool generateSixLines = true;

    // American roulette table layout
    private int[,] tableLayout = new int[12, 3]
    {
        {3, 2, 1},
        {6, 5, 4},
        {9, 8, 7},
        {12, 11, 10},
        {15, 14, 13},
        {18, 17, 16},
        {21, 20, 19},
        {24, 23, 22},
        {27, 26, 25},
        {30, 29, 28},
        {33, 32, 31},
        {36, 35, 34}
    };

    public void GenerateAllBetLocations()
    {
        if (tableParent == null)
        {
            Debug.LogError("Table Parent is not assigned!");
            return;
        }

        if (generateStraights) GenerateStraightBets();
        if (generateSplits) GenerateSplitBets();
        if (generateCorners) GenerateCornerBets();
        if (generateStreets) GenerateStreetBets();
        if (generateSixLines) GenerateSixLineBets();

        Debug.Log("Bet locations generated!");
    }

    public void ClearAllBetLocations()
    {
        if (tableParent == null) return;

        for (int i = tableParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tableParent.GetChild(i).gameObject);
        }

        Debug.Log("Bet locations cleared!");
    }

    private void GenerateStraightBets()
    {
        for (int col = 0; col < 12; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                int number = tableLayout[col, row];
                Vector3 position = CalculateCellPosition(col, row);

                CreateBetLocation(
                    $"Straight_{number}",
                    position,
                    straightBetSize,
                    BetType.Straight,
                    new List<int> { number },
                    35
                );
            }
        }

        // Add 0 and 00
        Vector3 zeroPosition = tableStartPosition + new Vector3(-cellWidth, 0, cellHeight * 1.5f);
        CreateBetLocation("Straight_0", zeroPosition, straightBetSize, BetType.Straight, new List<int> { 0 }, 35);

        Vector3 doubleZeroPosition = tableStartPosition + new Vector3(-cellWidth, 0, cellHeight * 0.5f);
        CreateBetLocation("Straight_00", doubleZeroPosition, straightBetSize, BetType.Straight, new List<int> { -1 }, 35);
    }

    private void GenerateSplitBets()
    {
        // Vertical splits
        for (int col = 0; col < 11; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                int num1 = tableLayout[col, row];
                int num2 = tableLayout[col + 1, row];

                Vector3 pos1 = CalculateCellPosition(col, row);
                Vector3 pos2 = CalculateCellPosition(col + 1, row);
                Vector3 splitPosition = (pos1 + pos2) / 2f;

                CreateBetLocation(
                    $"Split_V_{num1}_{num2}",
                    splitPosition,
                    splitBetSize,
                    BetType.Split,
                    new List<int> { num1, num2 },
                    17
                );
            }
        }

        // Horizontal splits
        for (int col = 0; col < 12; col++)
        {
            for (int row = 0; row < 2; row++)
            {
                int num1 = tableLayout[col, row];
                int num2 = tableLayout[col, row + 1];

                Vector3 pos1 = CalculateCellPosition(col, row);
                Vector3 pos2 = CalculateCellPosition(col, row + 1);
                Vector3 splitPosition = (pos1 + pos2) / 2f;

                Vector3 horizontalSplitSize = new Vector3(splitBetSize.z, splitBetSize.y, splitBetSize.x);

                CreateBetLocation(
                    $"Split_H_{num1}_{num2}",
                    splitPosition,
                    horizontalSplitSize,
                    BetType.Split,
                    new List<int> { num1, num2 },
                    17
                    );
            }
        }
    }

    private void GenerateCornerBets()
    {
        for (int col = 0; col < 11; col++)
        {
            for (int row = 0; row < 2; row++)
            {
                int num1 = tableLayout[col, row];
                int num2 = tableLayout[col + 1, row];
                int num3 = tableLayout[col, row + 1];
                int num4 = tableLayout[col + 1, row + 1];

                Vector3 pos1 = CalculateCellPosition(col, row);
                Vector3 pos2 = CalculateCellPosition(col + 1, row);
                Vector3 pos3 = CalculateCellPosition(col, row + 1);
                Vector3 pos4 = CalculateCellPosition(col + 1, row + 1);

                Vector3 cornerPosition = (pos1 + pos2 + pos3 + pos4) / 4f;

                CreateBetLocation(
                    $"Corner_{num1}_{num2}_{num3}_{num4}",
                    cornerPosition,
                    cornerBetSize,
                    BetType.Corner,
                    new List<int> { num1, num2, num3, num4 },
                    8
                );
            }
        }
    }

    private void GenerateStreetBets()
    {
        for (int col = 0; col < 12; col++)
        {
            int num1 = tableLayout[col, 0];
            int num2 = tableLayout[col, 1];
            int num3 = tableLayout[col, 2];

            Vector3 centerPos = CalculateCellPosition(col, 1);
            Vector3 streetPosition = centerPos + new Vector3(0, 0, cellHeight * 1.6f);

            CreateBetLocation(
                $"Street_{num1}_{num2}_{num3}",
                streetPosition,
                streetBetSize,
                BetType.Street,
                new List<int> { num1, num2, num3 },
                11
            );
        }
    }

    private void GenerateSixLineBets()
    {
        for (int col = 0; col < 11; col++)
        {
            int num1 = tableLayout[col, 0];
            int num2 = tableLayout[col, 1];
            int num3 = tableLayout[col, 2];
            int num4 = tableLayout[col + 1, 0];
            int num5 = tableLayout[col + 1, 1];
            int num6 = tableLayout[col + 1, 2];

            Vector3 pos1 = CalculateCellPosition(col, 1);
            Vector3 pos2 = CalculateCellPosition(col + 1, 1);
            Vector3 centerPos = (pos1 + pos2) / 2f;
            Vector3 sixLinePosition = centerPos + new Vector3(0, 0, cellHeight * 1.6f);

            CreateBetLocation(
                $"SixLine_{num1}_{num2}_{num3}_{num4}_{num5}_{num6}",
                sixLinePosition,
                sixLineBetSize,
                BetType.SixLine,
                new List<int> { num1, num2, num3, num4, num5, num6 },
                5
            );
        }
    }

    public Vector3 CalculateCellPosition(int col, int row)
    {
        float x = tableStartPosition.x + (col * cellWidth);
        float y = tableStartPosition.y;
        float z = tableStartPosition.z - (row * cellHeight);

        return new Vector3(x, y, z);
    }

    public void CreateBetLocation(string name, Vector3 position, Vector3 size, BetType betType,
                                   List<int> numbers, int payoutRatio)
    {
        GameObject betObj;

        if (betLocationPrefab != null)
        {
            betObj = Instantiate(betLocationPrefab, position, Quaternion.identity, tableParent);
        }
        else
        {
            betObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            betObj.transform.SetParent(tableParent);
            betObj.transform.position = position;
        }

        betObj.name = name;
        betObj.GetComponent<BoxCollider>().size = size;

        // Set layer
        int layer = LayerMask.NameToLayer(betLocationLayer);
        if (layer != -1)
            betObj.layer = layer;

        // Add or get BetLocation component
        BetLocation betLocation = betObj.GetComponent<BetLocation>();
        if (betLocation == null)
            betLocation = betObj.AddComponent<BetLocation>();

        // Configure bet location
        List<BetNumber> betNumbers = new List<BetNumber>();
        foreach (int num in numbers)
        {
            betNumbers.Add(new BetNumber(num));
        }
        betLocation.Configure(betType, betNumbers, payoutRatio);
    }
}