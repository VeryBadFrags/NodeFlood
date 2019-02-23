using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The core logic of the AI
public class Player
{

    public bool isHuman;
    public int aiLevel;
    public int team;
    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private static int AI_MEDIUM = 1;
    private static int AI_HARD = 2;
    private static int AI_VERY_HARD = 3;

    public GameObject PlaySomething(List<GameObject> nodesList)
    {
        GameObject nodeToPlay = null;

        if (aiLevel > AI_VERY_HARD)
        {
            if (nodeToPlay == null)
            {
                nodeToPlay = CaptureBiggestClusterOptimal(0, nodesList);

                if (nodeToPlay == null)
                {
                    nodeToPlay = CaptureBiggestClusterOptimal(-1, nodesList);
                }
            }
        }

        if (nodeToPlay == null)
        {
            // Play HARD
            if (aiLevel > AI_HARD)
            {
                nodeToPlay = CaptureBiggestCluster(0, nodesList); // TODO use constants + iterate through all players for 4 players mode

                if (nodeToPlay == null)
                {
                    nodeToPlay = CaptureBiggestCluster(-1, nodesList);
                }
            }

            if (nodeToPlay == null)
            {
                // PLAY MEDIUM
                if (aiLevel > AI_MEDIUM)
                {

                    nodeToPlay = CaptureInOne(0, nodesList);
                    if (nodeToPlay == null)
                    {
                        nodeToPlay = CaptureInOne(-1, nodesList);

                        if (nodeToPlay == null)
                        {
                            nodeToPlay = CaptureInTwo(0, nodesList);

                            if (nodeToPlay == null)
                            {
                                nodeToPlay = CaptureInTwo(-1, nodesList);
                            }
                        }
                    }
                }

                if (nodeToPlay == null)
                {
                    nodeToPlay = PlayDumb(nodesList);
                }
            }
        }

        return nodeToPlay;
    }

    /*
     * Plays the first available node
     */
    private GameObject PlayDumb(List<GameObject> nodesList)
    {
        foreach (GameObject candidateNode in nodesList)
        {
            if (candidateNode.GetComponent<Node>().owner == team)
            {
                return candidateNode;
            }
        }

        return null;
    }

    private GameObject CaptureInTwo(int targetTeam, List<GameObject> nodesList)
    {
        foreach (GameObject candidateNode in nodesList)
        {
            if (candidateNode.GetComponent<Node>().owner == team)
            {
                int newDirection = (candidateNode.GetComponent<Node>().direction + 2) % 4;
                GameObject destinationNode = TilesHelper.GetNeighbor(candidateNode, newDirection, nodesList);
                if (destinationNode != null)
                {
                    int targetOwner = destinationNode.GetComponent<Node>().owner;
                    if (targetOwner == targetTeam)
                    {
                        return candidateNode;
                    }
                }
            }
        }

        return null;
    }

    private GameObject CaptureInOne(int targetTeam, List<GameObject> nodesList)
    {
        foreach (GameObject candidateNode in nodesList)
        {
            if (candidateNode.GetComponent<Node>().owner == team)
            {
                int newDirection = (candidateNode.GetComponent<Node>().direction + 1) % 4;
                GameObject destinationNode = TilesHelper.GetNeighbor(candidateNode, newDirection, nodesList);
                if (destinationNode != null)
                {
                    int targetOwner = destinationNode.GetComponent<Node>().owner;
                    if (targetOwner == targetTeam)
                    {
                        return candidateNode;
                    }
                }
            }
        }

        return null;
    }

    private GameObject CaptureBiggestCluster(int targetTeam, List<GameObject> nodesList)
    {
        GameObject potentialBestNode = null;
        int bestClusterSize = 0;

        // For each node that belongs to the current player
        foreach (GameObject candidateNode in nodesList)
        {
            if (candidateNode.GetComponent<Node>().owner == team)
            {

                int clusterSize = TilesHelper.GetSizeOfTargetCluster(candidateNode, targetTeam, nodesList);

                if (clusterSize > bestClusterSize)
                {
                    potentialBestNode = candidateNode;
                    bestClusterSize = clusterSize;
                }
            }
        }

        return potentialBestNode;
    }

    private GameObject CaptureBiggestClusterOptimal(int targetTeam, List<GameObject> nodesList)
    {
        GameObject potentialBestNode = null;
        int bestClusterSize = 0;

        // For each node that belongs to the current player
        foreach (GameObject candidateNode in nodesList)
        {
            if (candidateNode.GetComponent<Node>().owner == team)
            {
                int clusterSize = TilesHelper.GetSizeOfTargetCluster(candidateNode, targetTeam, nodesList);

                if (clusterSize > bestClusterSize)
                {
                    potentialBestNode = candidateNode;
                    bestClusterSize = clusterSize;

                    bool cont = true;
                    int limit = 32;
                    while (cont && limit > 0)
                    {
                        cont = false;
                        limit--;
                        HashSet<GameObject> nodesBehind = TilesHelper.GetNodeBehind(potentialBestNode, nodesList);
                        foreach (GameObject nodeBehind in nodesBehind)
                        {
                            clusterSize = TilesHelper.GetSizeOfTargetCluster(nodeBehind, targetTeam, nodesList);
                            if (clusterSize >= bestClusterSize)
                            {
                                potentialBestNode = nodeBehind;
                                bestClusterSize = clusterSize;
                                cont = true;
                            }

                        }
                    }
                }
            }
        }

        return potentialBestNode;
    }

}
