using System;
using System.Collections.Generic;
using UnityEngine;


namespace XEngine.Engine
{
	public class LogPrinter : MonoSingleton<LogPrinter>
	{
		private struct Message
		{
			public string msg;

			public string trace;

			public LogType type;
		}

		public bool ForceEnabled = false;

		public KeyCode ToggleKey = KeyCode.Return;

		public bool ShakeEnabled = false;

		public float ShakeAcc = 3f;

		public bool IsLimit = true;

		public bool IsSenior = false;

		public int MaxCount = 100;

		private readonly List<Message> Messages = new();

		private Vector2 ScrollPos;

		private bool IsVisible = false;

		private bool IsCollapse = false;

		private static readonly Dictionary<LogType, Color> Colors = new()
		{
			{
				LogType.Log,
				Color.white
			},
			{
				LogType.Warning,
				Color.yellow
			},
			{
				LogType.Assert,
				Color.red
			},
			{
				LogType.Error,
				Color.red
			},
			{
				LogType.Exception,
				Color.red
			}
		};

		private const string WindowTitle = "System Debug Opened";

		private const int Margin = 20;

		private static readonly GUIContent ClearLabel = new("Clear", "Clear the Console");

		private static readonly GUIContent CollapseLabel = new("Collapse", "Hide the repeated messages");

		private static readonly GUIContent SeniorLabel = new("Senior", "Show advanced messages only");

		private readonly Rect TitleRect = new(0f, 0f, 10000f, 100f);

		private Rect WindowRect = new(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));

		private bool IsTouch = false;

		private bool IsDown = false;

		private Rect TouchArea;

		private Vector3 CurPos;

		private float ItemHeight = 100f;

		private float TouchTime = 0f;

		private readonly GUIStyle LabelStyle = new();

		protected void OnEnable()
		{
			Application.logMessageReceived += new Application.LogCallback(ReceiveMessage);
		}

		protected void OnDisable()
		{
			Application.logMessageReceived -= new Application.LogCallback(ReceiveMessage);
		}

		protected void Update()
		{
			if (Input.GetKeyDown(ToggleKey))
			{
				IsVisible = !IsVisible;
			}
			if (ShakeEnabled)
			{
				Vector3 acceleration = Input.acceleration;
				if (acceleration.sqrMagnitude > ShakeAcc)
				{
					IsVisible = !IsVisible;
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				IsDown = false;
				TouchArea = new Rect(0f, (float)Screen.height * 0.75f, (float)(Screen.width / 4), (float)Screen.height);
			}
			if (TouchArea.Contains(Input.mousePosition))
			{
				if (!IsTouch && Input.GetMouseButton(0))
				{
					TouchTime += Time.deltaTime;
					if (TouchTime > 1.2f)
					{
						IsVisible = !IsVisible;
						TouchTime = 0f;
						IsTouch = true;
						if (IsVisible)
						{
							ItemHeight = Screen.height / 32;
							float num = 0f;
							for (int i = 0; i < Messages.Count; i++)
							{
								Message message = Messages[i];
								if (IsCollapse && i > 0)
								{
									string msg = Messages[i - 1].msg;
									if (message.msg == msg)
									{
										continue;
									}
								}
								num += ItemHeight;
								if ((int)message.type == 4)
								{
									num += ItemHeight;
								}
							}
							ScrollPos = new Vector2(0f, num);
						}
					}
				}
				if (Input.GetMouseButtonUp(0))
				{
					TouchTime = 0f;
					IsTouch = false;
				}
			}
			if (ForceEnabled)
			{
				IsVisible = true;
			}
			if (IsVisible)
			{
				if (Input.touchCount > 0)
				{
					Touch touch = Input.GetTouch(0);
					if (!IsDown)
					{
						CurPos = new Vector3(touch.position.x, touch.position.y, 0);
						IsDown = true;
					}
				}
				else
				{
					IsDown = false;
				}
				if (IsDown)
				{
					Vector3 val = Input.mousePosition - CurPos;
					CurPos = Input.mousePosition;
					ScrollPos += new Vector2(0f, val.y);
				}
			}
			else
			{
				IsDown = false;
			}
		}

		protected void OnGUI()
		{
			if (IsVisible)
			{
				WindowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));
				WindowRect = GUILayout.Window(1, WindowRect, new GUI.WindowFunction(DrawWindow), WindowTitle, Array.Empty<GUILayoutOption>());
			}
		}

		private void DrawWindow(int windowId)
		{
			DrawLogList();
			DrawToolbar();
			GUI.DragWindow(TitleRect);
		}

		private void DrawLogList()
		{
			LabelStyle.fontSize = 25;
			ScrollPos = GUILayout.BeginScrollView(ScrollPos, Array.Empty<GUILayoutOption>());
			for (int i = 0; i < Messages.Count; i++)
			{
				Message message = Messages[i];
				if (IsCollapse && i > 0)
				{
					string msg = Messages[i - 1].msg;
					if (message.msg == msg)
					{
						continue;
					}
				}
				LabelStyle.normal.textColor = Colors[message.type];
				GUILayout.Label(message.msg, LabelStyle, Array.Empty<GUILayoutOption>());
				if ((int)message.type == 4)
				{
					GUILayout.Label(message.trace, Array.Empty<GUILayoutOption>());
				}
			}
			GUILayout.EndScrollView();
			GUI.contentColor = Color.white;
		}

		private void DrawToolbar()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUI.Button(new Rect(WindowRect.width * 0.5f - 30f, WindowRect.height - 70f, 80f, 50f), ClearLabel))
			{
				Messages.Clear();
			}
			IsCollapse = GUILayout.Toggle(IsCollapse, CollapseLabel, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
			IsSenior = GUILayout.Toggle(IsSenior, SeniorLabel, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
			GUILayout.EndHorizontal();
		}

		private void ReceiveMessage(string message, string trace, LogType type)
		{
			if (!IsSenior || (int)type != 3)
			{
				Messages.Add(new Message
				{
					msg = message,
					trace = trace,
					type = type
				});
				TrimExcessMessage();
			}
		}

		private void TrimExcessMessage()
		{
			if (IsLimit)
			{
				int num = Mathf.Max(Messages.Count - MaxCount, 0);
				if (num != 0 && !IsDown)
				{
					Messages.RemoveRange(0, num);
				}
			}
		}
	}
}