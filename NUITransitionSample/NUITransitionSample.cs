﻿using System;
using System.IO;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;

namespace FirstBottomRow
{
    class Program : NUIApplication
    {
        private readonly string[,] Keywords = new string[3, 2]
        {
            {"red", "redGrey"},
            {"green", "greenGrey"},
            {"blue", "blueGrey"}
        };
        private readonly string totalGreyTag = "totalGrey";

        private Navigator navigator;
        private ContentPage mainPage;
        private ContentPage redPage, greenPage, bluePage, totalPage;

        private readonly Color ColorGrey = new Color(0.82f, 0.80f, 0.78f, 1.0f);
        private readonly Color ColorBackground = new Color(0.99f, 0.94f, 0.83f, 1.0f);

        private readonly Color[] TileColor = { new Color("#F5625D"), new Color("#7DFF83"), new Color("#7E72DF") };
        private readonly Color[] PageColor = { new Color("#F5625D"), new Color("#7DFF83"), Color.Cyan };

        private readonly Vector2 baseSize = new Vector2(720.0f, 1280.0f);
        private Vector2 contentSize;
        private Vector2 windowSize;

        private float magnification;

        private float convertSize(float size)
        {
            return size * magnification;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        private void Initialize()
        {
            Window window = NUIApplication.GetDefaultWindow();
            window.BackgroundColor = Color.AntiqueWhite;
            window.KeyEvent += OnKeyEvent;
            bool makeRotation = false;
            if((float)(window.Size.Width) > (float)(window.Size.Height))
            {
                makeRotation = true;
                windowSize = new Vector2((float)(window.Size.Height), (float)(window.Size.Width));
            }
            else
            {
                windowSize = new Vector2((float)(window.Size.Width), (float)(window.Size.Height));
            }
            magnification = Math.Min(windowSize.X / baseSize.X, windowSize.Y / baseSize.Y);
            contentSize = baseSize * magnification;

            navigator = new Navigator()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(contentSize.Width, contentSize.Height),
                Transition = new Transition()
                {
                    TimePeriod = new TimePeriod(400),
                    AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                },
            };
            if(makeRotation)
            {
                navigator.Orientation = new Rotation(new Radian(new Degree(90.0f)), Vector3.ZAxis);
            }
            navigator.TransitionFinished += (object sender, EventArgs e) =>
            {
                navigator.Transition = new Transition()
                {
                    TimePeriod = new TimePeriod(400),
                    AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                };
            };
            window.Add(navigator);

            View mainRoot = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent
            };

            View layoutView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopCenter,
                ParentOrigin = ParentOrigin.TopCenter,
                Layout = new LinearLayout()
                {
                    LinearAlignment = LinearLayout.Alignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    CellPadding = new Size(convertSize(90), convertSize(90)),
                },
                Position = new Position(0, convertSize(90))
            };
            mainRoot.Add(layoutView);

            View redButton = CreateButton(0, redPage, new Radian(-(float)Math.PI / 2.0f), 0.0f);
            View greenButton = CreateButton(1, greenPage, new Radian(0.0f), 20.0f);
            View blueButton = CreateButton(2, bluePage, new Radian(0.0f), 0.0f);

            layoutView.Add(redButton);
            layoutView.Add(greenButton);
            layoutView.Add(blueButton);

            mainPage = new ContentPage()
            {
                Content = mainRoot,
            };
            navigator.Push(mainPage);

            View totalGreyView = new View()
            {
                Size = new Size(convertSize(75), convertSize(75)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionWithChild = true,
                    TransitionTag = totalGreyTag,
                }
            };

            View innerView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(60), convertSize(60)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.Wheat,
            };
            totalGreyView.Add(innerView);

            totalGreyView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.Transition = new Transition()
                    {
                        TimePeriod = new TimePeriod(800),
                        AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                    };
                    navigator.PushWithTransition(totalPage);
                }
                return true;
            };
            layoutView.Add(totalGreyView);


            // ------------------------------------------------------


            View totalPageRoot = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                SizeHeight = contentSize.Height,
            };

            View totalLayoutView = new View()
            {
                Layout = new GridLayout()
                {
                    Rows = 2,
                    GridOrientation = GridLayout.Orientation.Vertical,
                },
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
            };
            totalPageRoot.Add(totalLayoutView);

            for (int i = 0; i < 3; ++i)
            {
                View sizeView = new View()
                {
                    Size = new Size(contentSize.Width / 2.0f, contentSize.Height / 2.0f),
                };
                View smallView = CreatePageScene(i, (i==1)?20.0f:0.0f);
                smallView.Size = new Size(Math.Min(contentSize.Width, contentSize.Height), Math.Max(contentSize.Width, contentSize.Height));
                smallView.Scale = new Vector3(0.45f, 0.45f, 1.0f);
                sizeView.Add(smallView);
                totalLayoutView.Add(sizeView);
            }

            View sizeGreyView = new View()
            {
                Size = new Size(contentSize.Width / 2.0f, contentSize.Height / 2.0f),
            };

            View totalGreyReturnView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(105), convertSize(105)),
                CornerRadius = 0.28f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionWithChild = true,
                    TransitionTag = totalGreyTag,
                }
            };

            View innerReturnView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(60), convertSize(60)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.Wheat,
            };
            totalGreyReturnView.Add(innerReturnView);
            sizeGreyView.Add(totalGreyReturnView);
            totalLayoutView.Add(sizeGreyView);

            totalGreyReturnView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.Transition = new Transition()
                    {
                        TimePeriod = new TimePeriod(800),
                        AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                    };
                    navigator.PopWithTransition();
                }
                return true;
            };

            totalPage = new ContentPage()
            {
                Content = totalPageRoot,
            };
        }

        private View CreateButton(int index, Page secondPage, Radian angle, float shadowRadius)
        {
            Rotation rotation = new Rotation(angle, Vector3.ZAxis);
            View colorView = new View()
            {
                Size = new Size(convertSize(75), convertSize(75)),
                CornerRadius = convertSize(34),
                BackgroundColor = TileColor[index],
                Orientation = rotation,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 0],
                },
            };

            rotation.Invert();
            View greyView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(40), convertSize(40)),
                CornerRadius = 0.45f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                Orientation = rotation,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 1],
                }
            };

            if(shadowRadius > 0.0f)
            {
                greyView.BoxShadow = new Shadow(shadowRadius, null);
                greyView.InheritScale = false;
            }

            secondPage = CreatePage(index, shadowRadius);

            colorView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.PushWithTransition(secondPage);
                }
                return true;
            };
            colorView.Add(greyView);
            return colorView;
        }

        private View CreatePageScene(int index, float shadowRadius)
        {
            View pageBackground = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent
            };

            View colorView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                BackgroundColor = PageColor[index],
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 0]
                }
            };

            View greyView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopCenter,
                ParentOrigin = ParentOrigin.TopCenter,
                Position = new Position(0, convertSize(120)),
                SizeWidth = contentSize.Width * 0.35f,
                SizeHeight = contentSize.Height * 0.03f,
                Scale = new Vector3(2.0f, 2.0f, 1.0f),
                CornerRadius = 0.1f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 1]
                }
            };

            if(shadowRadius > 0.0f)
            {
                greyView.BoxShadow = new Shadow(shadowRadius, null);
            }

            View whiteView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomCenter,
                ParentOrigin = ParentOrigin.BottomCenter,
                Position = new Position(0, -convertSize(90)),
                SizeWidth = contentSize.Width * 0.65f,
                SizeHeight = contentSize.Height * 0.7f,
                CornerRadius = 0.1f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.AntiqueWhite,
            };
            pageBackground.Add(colorView);
            pageBackground.Add(whiteView);
            pageBackground.Add(greyView);

            return pageBackground;
        }

        private Page CreatePage(int index, float shadowRadius)
        {
            View pageRoot = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(windowSize.Width, windowSize.Height),
            };

            View pageBackground = CreatePageScene(index, shadowRadius);
            pageBackground.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.PopWithTransition();
                }
                return true;
            };
            pageRoot.Add(pageBackground);

            Page page = new ContentPage()
            {
                Content = pageRoot,
            };
            return page;
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                if(navigator.PageCount == 1)
                {
                    Exit();
                }
                else
                {
                    navigator.PopWithTransition();
                }
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}