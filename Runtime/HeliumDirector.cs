using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Martian.Helium
{
    /// <summary>
    /// The main class that handles cutscene and dialogue direction.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class HeliumDirector : MonoBehaviour
    {
        public static HeliumDirector Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// The current helium runtime graph being executed.
        /// </summary>
        public HeliumRuntimeGraph CurrentGraph { get; private set; }
        public bool IsRunning { get; private set; }

        private CancellationTokenSource _currentGraphCancellation;

        [Header("Input")]
        public HeliumInput Input { get; private set; }

        [Header("Views")]
        [SerializeField] private List<HeliumView> _viewList = new();
        public Dictionary<string, string> CurrentViewInfo { get; private set; }

        #region Events

        public delegate void GraphEvent();
        public event GraphEvent OnGraphStarted, OnGraphComplete;

        #endregion

        private void Start()
        {
            // Set starting view info
            UpdateViewsInfo(new Dictionary<string, string>());

            // Get input
            Input = GetComponentInChildren<HeliumInput>();

            if(Input == null )
            {
                Debug.LogError("No Helium Input found under Director.");
            }
        }

        public async void StartGraph( HeliumRuntimeGraph graph )
        {
            if (IsRunning) { return; }

            // start the graph awaitable with a cancelation token so we can stop it midway through
            _currentGraphCancellation = new CancellationTokenSource();
            await PlayGraph(graph, _currentGraphCancellation.Token);
        }

        public void StopGraph()
        {
            if (!IsRunning) { return; }

            IsRunning = false;
            OnGraphComplete?.Invoke();

            // disable graph input
            Input.DisableInput();

            _currentGraphCancellation.Cancel();
            _currentGraphCancellation = null;

            // clear views info
            ClearViewsInfo();

            Debug.Log("Helium Graph Stopped!");

        }

        private void GraphComplete()
        {
            if (!IsRunning) { return; }

            // clear views info
            ClearViewsInfo();

            // disable graph input
            Input.DisableInput();

            IsRunning = false;
            OnGraphComplete?.Invoke();

            Debug.Log("Helium Graph Complete!");

        }

        private async Awaitable PlayGraph(HeliumRuntimeGraph graph, CancellationToken token)
        {
            IsRunning = true;
            OnGraphStarted?.Invoke();
            Debug.Log("Helium Graph Started!");

            // enable the input for the graph
            Input.EnableInput();

            // create all the executors needed
            var setDialogueExecutor = new SetDialogueExecutor();
            var waitForInputExecutor = new WaitForInputExecutor();

            // TODO: Remove this and instead place a while loop that goes through each nodes out until there are no nodes left
            var currentNode = graph.StartNode;

            while (currentNode != null)
            {
                // if we are no longer running aka stopped, we won't do anything else
                if (!IsRunning) { break; }

                switch (currentNode)
                {
                    case SetDialogueRuntimeNode dialogueNode:
                        await setDialogueExecutor.ExecuteNodeAsync(dialogueNode, this, token);
                        break;
                    case WaitForInputRuntimeNode waitInputNode:
                        await waitForInputExecutor.ExecuteNodeAsync(waitInputNode, this, token);
                        break;
                    default:
                        Debug.LogError($"No executor found for node type: {currentNode.GetType()}");
                        break;
                }

                if (currentNode.OutputNodes.Count <= 0)
                {
                    break;
                }

                currentNode = graph.FindNodeFromGUID(currentNode.OutputNodes[0]);

            }

            // mark the graph complete
            GraphComplete();
        }

        #region Dialogue
        [Header("Dialogue Settings")]
        public float GlobalCharDelayTime = 0.02f;
        public float GlobalPuncDelayTime = 0.05f;

        public void UpdateViewsInfo(Dictionary<string, string> viewsInfo)
        {
            CurrentViewInfo = viewsInfo;

            // Implementation for updating view info
            foreach (var view in _viewList)
            {
                view.UpdateInfo(CurrentViewInfo);
            }
        }

        public void ClearViewsInfo()
        {
            CurrentViewInfo.Clear();

            foreach (var view in _viewList)
            {
                view.UpdateInfo(CurrentViewInfo);
            }
        }

        #endregion

    }
}