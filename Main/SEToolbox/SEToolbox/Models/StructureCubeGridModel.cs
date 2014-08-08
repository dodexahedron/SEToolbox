﻿namespace SEToolbox.Models
{
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Common.ObjectBuilders.Definitions;
    using Sandbox.Common.ObjectBuilders.VRageData;
    using SEToolbox.Interop;
    using SEToolbox.Support;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using VRageMath;

    [Serializable]
    public class StructureCubeGridModel : StructureBaseModel
    {
        #region Fields

        // Fields are marked as NonSerialized, as they aren't required during the drag-drop operation.

        [NonSerialized]
        private Point3D _min;

        [NonSerialized]
        private Point3D _max;

        [NonSerialized]
        private Vector3D _scale;

        [NonSerialized]
        private Size3D _size;

        [NonSerialized]
        private int _pilots;

        [NonSerialized]
        private TimeSpan _timeToProduce;

        [NonSerialized]
        private string _cockpitOrientation;

        [NonSerialized]
        private List<CubeAssetModel> _cubeAssets;

        [NonSerialized]
        private List<CubeAssetModel> _componentAssets;

        [NonSerialized]
        private List<OreAssetModel> _ingotAssets;

        [NonSerialized]
        private List<OreAssetModel> _oreAssets;

        [NonSerialized]
        private string _activeComponentFilter;

        [NonSerialized]
        private string _componentFilter;

        [NonSerialized]
        private ObservableCollection<CubeItemModel> _cubeList;

        [NonSerialized]
        private static readonly object Locker = new object();

        [NonSerialized]
        private bool _isSubsSystemNotReady;

        [NonSerialized]
        private bool _isConstructionNotReady;

        #endregion

        #region ctor

        public StructureCubeGridModel(MyObjectBuilder_EntityBase entityBase, MySessionSettings settings)
            : base(entityBase, settings)
        {
            this.IsSubsSystemNotReady = true;
            this.IsConstructionNotReady = true;
        }

        #endregion

        #region Properties

        public MyObjectBuilder_CubeGrid CubeGrid
        {
            get
            {
                return this.EntityBase as MyObjectBuilder_CubeGrid;
            }
        }

        public Sandbox.Common.ObjectBuilders.MyCubeSize GridSize
        {
            get
            {
                return this.CubeGrid.GridSizeEnum;
            }

            set
            {
                if (value != this.CubeGrid.GridSizeEnum)
                {
                    this.CubeGrid.GridSizeEnum = value;
                    this.RaisePropertyChanged(() => GridSize);
                }
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.CubeGrid.IsStatic;
            }

            set
            {
                if (value != this.CubeGrid.IsStatic)
                {
                    this.CubeGrid.IsStatic = value;
                    this.RaisePropertyChanged(() => IsStatic);
                }
            }
        }

        public bool Dampeners
        {
            get
            {
                return this.CubeGrid.DampenersEnabled;
            }

            set
            {
                if (value != this.CubeGrid.DampenersEnabled)
                {
                    this.CubeGrid.DampenersEnabled = value;
                    this.RaisePropertyChanged(() => Dampeners);
                }
            }
        }

        public Point3D Min
        {
            get
            {
                return this._min;
            }

            set
            {
                if (value != this._min)
                {
                    this._min = value;
                    this.RaisePropertyChanged(() => Min);
                }
            }
        }

        public Point3D Max
        {
            get
            {
                return this._max;
            }

            set
            {
                if (value != this._max)
                {
                    this._max = value;
                    this.RaisePropertyChanged(() => Max);
                }
            }
        }

        public Vector3D Scale
        {
            get
            {
                return this._scale;
            }

            set
            {
                if (value != this._scale)
                {
                    this._scale = value;
                    this.RaisePropertyChanged(() => Scale);
                }
            }
        }

        public Size3D Size
        {
            get
            {
                return this._size;
            }

            set
            {
                if (value != this._size)
                {
                    this._size = value;
                    this.RaisePropertyChanged(() => Size);
                }
            }
        }

        public int Pilots
        {
            get
            {
                return this._pilots;
            }

            set
            {
                if (value != this._pilots)
                {
                    this._pilots = value;
                    this.RaisePropertyChanged(() => Pilots);
                }
            }
        }

        public bool IsPiloted
        {
            get
            {
                return this.Pilots > 0;
            }
        }

        public bool IsDamaged
        {
            get
            {
                // TODO: check the CubeBlocks/ cube.IntegrityPercent
                return true; //this.CubeGrid.Skeleton.Count > 0;
            }
        }

        public int DamageCount
        {
            get
            {
                // TODO: create a seperate property for the CubeBlocks/ cube.IntegrityPercent
                return this.CubeGrid.Skeleton.Count;
            }
        }

        public double LinearVelocity
        {
            get
            {
                return this.CubeGrid.LinearVelocity.ToVector3().LinearVector();
            }
        }

        /// This is not to be taken as an accurate representation.
        public double AngularSpeed
        {
            get
            {
                return this.CubeGrid.AngularVelocity.ToVector3().LinearVector();
            }
        }

        public TimeSpan TimeToProduce
        {
            get
            {
                return this._timeToProduce;
            }

            set
            {
                if (value != this._timeToProduce)
                {
                    this._timeToProduce = value;
                    this.RaisePropertyChanged(() => TimeToProduce);
                }
            }
        }

        public override int BlockCount
        {
            get
            {
                return this.CubeGrid.CubeBlocks.Count;
            }
        }

        public string CockpitOrientation
        {
            get
            {
                return this._cockpitOrientation;
            }

            set
            {
                if (value != this._cockpitOrientation)
                {
                    this._cockpitOrientation = value;
                    this.RaisePropertyChanged(() => CockpitOrientation);
                }
            }
        }

        /// <summary>
        /// This is detail of the breakdown of cubes in the ship.
        /// </summary>
        public List<CubeAssetModel> CubeAssets
        {
            get
            {
                return this._cubeAssets;
            }

            set
            {
                if (value != this._cubeAssets)
                {
                    this._cubeAssets = value;
                    this.RaisePropertyChanged(() => CubeAssets);
                }
            }
        }

        /// <summary>
        /// This is detail of the breakdown of components in the ship.
        /// </summary>
        public List<CubeAssetModel> ComponentAssets
        {
            get
            {
                return this._componentAssets;
            }

            set
            {
                if (value != this._componentAssets)
                {
                    this._componentAssets = value;
                    this.RaisePropertyChanged(() => ComponentAssets);
                }
            }
        }

        /// <summary>
        /// This is detail of the breakdown of ingots in the ship.
        /// </summary>
        public List<OreAssetModel> IngotAssets
        {
            get
            {
                return this._ingotAssets;
            }

            set
            {
                if (value != this._ingotAssets)
                {
                    this._ingotAssets = value;
                    this.RaisePropertyChanged(() => IngotAssets);
                }
            }
        }

        /// <summary>
        /// This is detail of the breakdown of ore in the ship.
        /// </summary>
        public List<OreAssetModel> OreAssets
        {
            get
            {
                return this._oreAssets;
            }

            set
            {
                if (value != this._oreAssets)
                {
                    this._oreAssets = value;
                    this.RaisePropertyChanged(() => OreAssets);
                }
            }
        }

        public string ActiveComponentFilter
        {
            get
            {
                return this._activeComponentFilter;
            }

            set
            {
                if (value != this._activeComponentFilter)
                {
                    this._activeComponentFilter = value;
                    this.RaisePropertyChanged(() => ActiveComponentFilter);
                }
            }
        }

        public string ComponentFilter
        {
            get
            {
                return this._componentFilter;
            }

            set
            {
                if (value != this._componentFilter)
                {
                    this._componentFilter = value;
                    this.RaisePropertyChanged(() => ComponentFilter);
                }
            }
        }

        public ObservableCollection<CubeItemModel> CubeList
        {
            get
            {
                return this._cubeList;
            }

            set
            {
                if (value != this._cubeList)
                {
                    this._cubeList = value;
                    this.RaisePropertyChanged(() => CubeList);
                }
            }
        }

        public bool IsSubsSystemNotReady
        {
            get { return this._isSubsSystemNotReady; }

            set
            {
                if (value != this._isSubsSystemNotReady)
                {
                    this._isSubsSystemNotReady = value;
                    this.RaisePropertyChanged(() => IsSubsSystemNotReady);
                }
            }
        }

        public bool IsConstructionNotReady
        {
            get { return this._isConstructionNotReady; }

            set
            {
                if (value != this._isConstructionNotReady)
                {
                    this._isConstructionNotReady = value;
                    this.RaisePropertyChanged(() => IsConstructionNotReady);
                }
            }
        }

        #endregion

        #region methods

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            this.SerializedEntity = SpaceEngineersApi.Serialize<MyObjectBuilder_CubeGrid>(this.CubeGrid);
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.EntityBase = SpaceEngineersApi.Deserialize<MyObjectBuilder_CubeGrid>(this.SerializedEntity);
        }

        public override void UpdateGeneralFromEntityBase()
        {
            double scaleMultiplyer = this.CubeGrid.GridSizeEnum.ToLength();
            if (this.IsStatic && this.CubeGrid.GridSizeEnum == MyCubeSize.Large)
            {
                this.ClassType = ClassType.Station;
            }
            else if (!this.IsStatic && this.CubeGrid.GridSizeEnum == MyCubeSize.Large)
            {
                this.ClassType = ClassType.LargeShip;
            }
            else if (!this.IsStatic && this.CubeGrid.GridSizeEnum == MyCubeSize.Small)
            {
                this.ClassType = ClassType.SmallShip;
            }

            var min = new Point3D(int.MaxValue, int.MaxValue, int.MaxValue);
            var max = new Point3D(int.MinValue, int.MinValue, int.MinValue);
            float totalMass = 0;

            foreach (var block in this.CubeGrid.CubeBlocks)
            {
                min.X = Math.Min(min.X, block.Min.X);
                min.Y = Math.Min(min.Y, block.Min.Y);
                min.Z = Math.Min(min.Z, block.Min.Z);

                var cubeDefinition = SpaceEngineersApi.GetCubeDefinition(block.TypeId, this.CubeGrid.GridSizeEnum, block.SubtypeName);

                // definition is null when the block no longer exists in the Cube definitions. Ie, Ladder, or a Mod that was removed.
                if (cubeDefinition == null || (cubeDefinition.Size.X == 1 && cubeDefinition.Size.Y == 1 && cubeDefinition.Size.z == 1))
                {
                    max.X = Math.Max(max.X, block.Min.X);
                    max.Y = Math.Max(max.Y, block.Min.Y);
                    max.Z = Math.Max(max.Z, block.Min.Z);
                }
                else
                {
                    // resolve the cube size acording to the cube's orientation.
                    var orientSize = cubeDefinition.Size.Add(-1).Transform(block.BlockOrientation).Abs();
                    max.X = Math.Max(max.X, block.Min.X + orientSize.X);
                    max.Y = Math.Max(max.Y, block.Min.Y + orientSize.Y);
                    max.Z = Math.Max(max.Z, block.Min.Z + orientSize.Z);
                }

                var cubeBlockDefinition = SpaceEngineersApi.GetCubeDefinition(block.TypeId, this.CubeGrid.GridSizeEnum, block.SubtypeName);

                float cubeMass = 0;
                if (cubeBlockDefinition != null)
                {
                    foreach (var component in cubeBlockDefinition.Components)
                    {
                        var cd = SpaceEngineersApi.GetDefinition(component.Type, component.Subtype) as MyObjectBuilder_ComponentDefinition;
                        float componentMass = cd.Mass * component.Count;
                        cubeMass += componentMass;
                    }
                }

                totalMass += cubeMass;
            }

            var cockpitOrientation = "None";
            var cockpits = this.CubeGrid.CubeBlocks.Where(b => b is MyObjectBuilder_Cockpit).ToArray();
            if (cockpits.Length > 0)
            {
                var count = cockpits.Count(b => b.BlockOrientation.Forward == cockpits[0].BlockOrientation.Forward && b.BlockOrientation.Up == cockpits[0].BlockOrientation.Up);
                if (cockpits.Length == count)
                {
                    // All cockpits share the same orientation.
                    cockpitOrientation = string.Format("Forward={0} ({1}), Up={2} ({3})", cockpits[0].BlockOrientation.Forward, GetAxisIndicator(cockpits[0].BlockOrientation.Forward), cockpits[0].BlockOrientation.Up, GetAxisIndicator(cockpits[0].BlockOrientation.Up));
                }
                else
                {
                    // multiple cockpits are present, and do not share a common orientation.
                    cockpitOrientation = "Mixed";
                }
            }
            this.CockpitOrientation = cockpitOrientation;

            var scale = max - min;
            scale.X++;
            scale.Y++;
            scale.Z++;

            this.Min = min;
            this.Max = max;
            this.Scale = scale;
            this.Size = new Size3D(scale.X * scaleMultiplyer, scale.Y * scaleMultiplyer, scale.Z * scaleMultiplyer);
            this.Mass = totalMass;

            this.DisplayName = null;
            // Substitue Beacon or Antenna detail for the DisplayName.
            var bradcasters = this.CubeGrid.CubeBlocks.Where(b => b.SubtypeName == SubtypeId.LargeBlockBeacon.ToString()
                || b.SubtypeName == SubtypeId.SmallBlockBeacon.ToString()
                || b.SubtypeName == SubtypeId.LargeBlockRadioAntenna.ToString()
                || b.SubtypeName == SubtypeId.SmallBlockRadioAntenna.ToString()).ToArray();
            if (bradcasters.Length > 0)
            {
                var beaconNames = bradcasters.Where(b => b is MyObjectBuilder_Beacon).Select(b => ((MyObjectBuilder_Beacon)b).CustomName ?? "Beacon").ToArray();
                var antennaNames = bradcasters.Where(b => b is MyObjectBuilder_RadioAntenna).Select(b => ((MyObjectBuilder_RadioAntenna)b).CustomName ?? "Antenna").ToArray();
                this.DisplayName = String.Join("|", beaconNames.Concat(antennaNames).OrderBy(s => s));
            }

            this.Description = string.Format("{0}×{1}×{2}", this.Scale.X, this.Scale.Y, this.Scale.Z);

            // TODO:
            // Report:
            // Reflectors On
            // Mass:      9,999,999 Kg
            // Speed:          0.0 m/s
            // Power Usage:      0.05%
            // Reactors:     12,999 GW
            // Thrusts:            999
            // Gyros:              999
            // Fuel Time:        0 sec
        }

        public override void InitializeAsync()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += delegate(object s, DoWorkEventArgs workArgs)
            {
                lock (Locker)
                {
                    // Because I've bound models to the view, this is going to get messy.
                    var contentPath = ToolboxUpdater.GetApplicationContentPath();

                    if (this.IsConstructionNotReady)
                    {
                        var ingotRequirements = new Dictionary<string, MyObjectBuilder_BlueprintDefinition.Item>();
                        var oreRequirements = new Dictionary<string, MyObjectBuilder_BlueprintDefinition.Item>();
                        var timeTaken = new TimeSpan();
                        var cubeAssetDict = new Dictionary<string, CubeAssetModel>();
                        var componentAssetDict = new Dictionary<string, CubeAssetModel>();
                        var cubeAssets = new List<CubeAssetModel>();
                        var componentAssets = new List<CubeAssetModel>();
                        var ingotAssets = new List<OreAssetModel>();
                        var oreAssets = new List<OreAssetModel>();

                        foreach (var block in this.CubeGrid.CubeBlocks)
                        {
                            var blockName = block.SubtypeName;
                            if (string.IsNullOrEmpty(blockName))
                            {
                                blockName = block.TypeId.ToString();
                            }

                            var cubeBlockDefinition = SpaceEngineersApi.GetCubeDefinition(block.TypeId, this.CubeGrid.GridSizeEnum, block.SubtypeName);

                            float cubeMass = 0;
                            TimeSpan blockTime = TimeSpan.Zero;
                            string blockTexture = null;

                            if (cubeBlockDefinition != null)
                            {
                                foreach (var component in cubeBlockDefinition.Components)
                                {
                                    TimeSpan componentTime;
                                    SpaceEngineersApi.AccumulateCubeBlueprintRequirements(component.Subtype, component.Type, component.Count, ingotRequirements, out componentTime);
                                    timeTaken += componentTime;

                                    var cd = SpaceEngineersApi.GetDefinition(component.Type, component.Subtype) as MyObjectBuilder_ComponentDefinition;
                                    float componentMass = cd.Mass * component.Count;
                                    float componentVolume = cd.Volume.Value * component.Count;
                                    cubeMass += componentMass;

                                    var componentName = component.Subtype;
                                    if (componentAssetDict.ContainsKey(componentName))
                                    {
                                        componentAssetDict[componentName].Count += component.Count;
                                        componentAssetDict[componentName].Mass += componentMass;
                                        componentAssetDict[componentName].Volume += componentVolume;
                                        componentAssetDict[componentName].Time += componentTime;
                                    }
                                    else
                                    {
                                        var componentTexture = SpaceEngineersCore.GetDataPathOrDefault(cd.Icon, Path.Combine(contentPath, cd.Icon));
                                        var m = new CubeAssetModel() { Name = cd.DisplayName, Mass = componentMass, Volume = componentVolume, Count = component.Count, Time = componentTime, TextureFile = componentTexture };
                                        componentAssets.Add(m);
                                        componentAssetDict.Add(componentName, m);
                                    }
                                }

                                blockTime = new TimeSpan((long)(TimeSpan.TicksPerSecond * cubeBlockDefinition.BuildTimeSeconds));
                                blockTexture = SpaceEngineersCore.GetDataPathOrDefault(cubeBlockDefinition.Icon, Path.Combine(contentPath, cubeBlockDefinition.Icon));
                            }

                            timeTaken += blockTime;

                            if (cubeAssetDict.ContainsKey(blockName))
                            {
                                cubeAssetDict[blockName].Count++;
                                cubeAssetDict[blockName].Mass += cubeMass;
                                cubeAssetDict[blockName].Time += blockTime;
                            }
                            else
                            {
                                var m = new CubeAssetModel() { Name = cubeBlockDefinition == null ? blockName : cubeBlockDefinition.DisplayName, Mass = cubeMass, Count = 1, TextureFile = blockTexture, Time = blockTime };
                                cubeAssets.Add(m);
                                cubeAssetDict.Add(blockName, m);
                            }
                        }

                        foreach (var kvp in ingotRequirements)
                        {
                            TimeSpan ingotTime;
                            SpaceEngineersApi.AccumulateCubeBlueprintRequirements(kvp.Value.SubtypeId, kvp.Value.Id.TypeId, kvp.Value.Amount, oreRequirements, out ingotTime);
                            var cd = SpaceEngineersApi.GetDefinition(kvp.Value.Id.TypeId, kvp.Value.SubtypeId) as MyObjectBuilder_PhysicalItemDefinition;
                            var componentTexture = SpaceEngineersCore.GetDataPathOrDefault(cd.Icon, Path.Combine(contentPath, cd.Icon));
                            var ingotAsset = new OreAssetModel() { Name = cd.DisplayName, Amount = kvp.Value.Amount, Mass = (double)kvp.Value.Amount * cd.Mass, Volume = (double)kvp.Value.Amount * cd.Volume.Value, Time = ingotTime, TextureFile = componentTexture };
                            ingotAssets.Add(ingotAsset);
                            timeTaken += ingotTime;
                        }

                        foreach (var kvp in oreRequirements)
                        {
                            var cd = SpaceEngineersApi.GetDefinition(kvp.Value.Id.TypeId, kvp.Value.SubtypeId) as MyObjectBuilder_PhysicalItemDefinition;
                            var componentTexture = SpaceEngineersCore.GetDataPathOrDefault(cd.Icon, Path.Combine(contentPath, cd.Icon));
                            var oreAsset = new OreAssetModel() { Name = cd.DisplayName, Amount = kvp.Value.Amount, Mass = (double)kvp.Value.Amount * cd.Mass, Volume = (double)kvp.Value.Amount * cd.Volume.Value, TextureFile = componentTexture };
                            oreAssets.Add(oreAsset);
                        }

                        this._dispatcher.Invoke(DispatcherPriority.Input, (Action)delegate
                        {
                            this.CubeAssets = cubeAssets;
                            this.ComponentAssets = componentAssets;
                            this.IngotAssets = ingotAssets;
                            this.OreAssets = oreAssets;
                            this.TimeToProduce = timeTaken;
                        });

                        this.IsConstructionNotReady = false;
                    }

                    if (this.IsSubsSystemNotReady)
                    {
                        var cubeList = new List<CubeItemModel>();

                        foreach (var block in this.CubeGrid.CubeBlocks)
                        {
                            var cubeDefinition = SpaceEngineersApi.GetCubeDefinition(block.TypeId, this.CubeGrid.GridSizeEnum, block.SubtypeName);

                            this._dispatcher.Invoke(DispatcherPriority.Input, (Action)delegate
                            {
                                cubeList.Add(new CubeItemModel(block, cubeDefinition, Settings)
                                {
                                    TextureFile = cubeDefinition == null ? null : SpaceEngineersCore.GetDataPathOrDefault(cubeDefinition.Icon, Path.Combine(contentPath, cubeDefinition.Icon))
                                });
                            });
                        }

                        this._dispatcher.Invoke(DispatcherPriority.Input, (Action)delegate
                        {
                            this.CubeList = new ObservableCollection<CubeItemModel>(cubeList);
                        });

                        this.IsSubsSystemNotReady = false;
                    }
                }
            };

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Find any Cockpits that have player character/s in them.
        /// </summary>
        /// <returns></returns>
        public List<MyObjectBuilder_Cockpit> GetActiveCockpits()
        {
            var cubes = this.CubeGrid.CubeBlocks.Where<MyObjectBuilder_CubeBlock>(e => e is MyObjectBuilder_Cockpit && ((MyObjectBuilder_Cockpit)e).Pilot != null);
            return new List<MyObjectBuilder_Cockpit>(cubes.Cast<MyObjectBuilder_Cockpit>());
        }

        public void RepairAllDamage()
        {
            if (this.CubeGrid.Skeleton == null)
                this.CubeGrid.Skeleton = new System.Collections.Generic.List<BoneInfo>();
            else
                this.CubeGrid.Skeleton.Clear();

            foreach (var cube in this.CubeGrid.CubeBlocks)
            {
                cube.IntegrityPercent = cube.BuildPercent;
            }

            this.RaisePropertyChanged(() => IsDamaged);
            this.RaisePropertyChanged(() => DamageCount);
        }

        public void ResetVelocity()
        {
            this.CubeGrid.LinearVelocity = new VRageMath.Vector3(0, 0, 0);
            this.CubeGrid.AngularVelocity = new VRageMath.Vector3(0, 0, 0);
            this.RaisePropertyChanged(() => LinearVelocity);
        }

        public void ReverseVelocity()
        {
            this.CubeGrid.LinearVelocity = new VRageMath.Vector3(this.CubeGrid.LinearVelocity.X * -1, this.CubeGrid.LinearVelocity.Y * -1, this.CubeGrid.LinearVelocity.Z * -1);
            this.CubeGrid.AngularVelocity = new VRageMath.Vector3(this.CubeGrid.AngularVelocity.X * -1, this.CubeGrid.AngularVelocity.Y * -1, this.CubeGrid.AngularVelocity.Z * -1);
            this.RaisePropertyChanged(() => LinearVelocity);
        }

        public void MaxVelocityAtPlayer(Vector3 playerPosition)
        {
            var v = playerPosition - this.CubeGrid.PositionAndOrientation.Value.Position;
            v.Normalize();
            v = Vector3.Multiply(v, SpaceEngineersConsts.MaxShipVelocity);

            this.CubeGrid.LinearVelocity = v;
            this.CubeGrid.AngularVelocity = new VRageMath.Vector3(0, 0, 0);
            this.RaisePropertyChanged(() => LinearVelocity);
        }

        public bool ConvertFromLightToHeavyArmor()
        {
            var count = 0;
            foreach (var cube in this.CubeGrid.CubeBlocks)
            {
                if (cube.SubtypeName.StartsWith("LargeBlockArmor"))
                {
                    cube.SubtypeName = cube.SubtypeName.Replace("LargeBlockArmor", "LargeHeavyBlockArmor");
                    count++;
                }
                else if (cube.SubtypeName.StartsWith("SmallBlockArmor"))
                {
                    cube.SubtypeName = cube.SubtypeName.Replace("SmallBlockArmor", "SmallHeavyBlockArmor");
                    count++;
                }
            }

            this.UpdateGeneralFromEntityBase();
            return count > 0;
        }

        public bool ConvertFromHeavyToLightArmor()
        {
            var count = 0;
            foreach (var cube in this.CubeGrid.CubeBlocks)
            {
                if (cube.SubtypeName.StartsWith("LargeHeavyBlockArmor"))
                {
                    cube.SubtypeName = cube.SubtypeName.Replace("LargeHeavyBlockArmor", "LargeBlockArmor");
                    count++;
                }
                else if (cube.SubtypeName.StartsWith("SmallHeavyBlockArmor"))
                {
                    cube.SubtypeName = cube.SubtypeName.Replace("SmallHeavyBlockArmor", "SmallBlockArmor");
                    count++;
                }
            }

            this.UpdateGeneralFromEntityBase();
            return count > 0;
        }

        public void ConvertToFramework(float value)
        {
            foreach (var cube in this.CubeGrid.CubeBlocks)
            {
                cube.IntegrityPercent = value;
                cube.BuildPercent = value;
            }

            this.UpdateGeneralFromEntityBase();
        }

        public void ConvertToStation()
        {
            this.ResetVelocity();
            this.CubeGrid.IsStatic = true;
            this.UpdateGeneralFromEntityBase();
        }

        public void ReorientStation()
        {
            var pos = this.CubeGrid.PositionAndOrientation.Value;
            pos.Position = pos.Position.RoundOff(MyCubeSize.Large.ToLength());
            pos.Forward = new SerializableVector3(-1, 0, 0); // The Station orientation has to be fixed, otherwise it glitches when you copy the object in game.
            pos.Up = new SerializableVector3(0, 1, 0);
            this.CubeGrid.PositionAndOrientation = pos;
        }

        public void RotateCubes(VRageMath.Quaternion quaternion)
        {
            foreach (var cube in this.CubeGrid.CubeBlocks)
            {
                var definition = SpaceEngineersApi.GetCubeDefinition(cube.TypeId, this.CubeGrid.GridSizeEnum, cube.SubtypeName);

                if (definition.Size.X == 1 && definition.Size.Y == 1 && definition.Size.z == 1)
                {
                    // rotate position around origin.
                    cube.Min = Vector3I.Transform(cube.Min.ToVector3I(), quaternion);
                }
                else
                {
                    // resolve size of component, and transform to original orientation.
                    var orientSize = definition.Size.Add(-1).Transform(cube.BlockOrientation).Abs();

                    var min = Vector3I.Transform(cube.Min.ToVector3I(), quaternion);
                    var blockMax = new Vector3I(cube.Min.X + orientSize.X, cube.Min.Y + orientSize.Y, cube.Min.Z + orientSize.Z);
                    var max = Vector3I.Transform(blockMax, quaternion);

                    cube.Min = new SerializableVector3I(Math.Min(min.X, max.X), Math.Min(min.Y, max.Y), Math.Min(min.Z, max.Z));
                }

                // rotate BlockOrientation.
                var q = quaternion * cube.BlockOrientation.ToQuaternion();
                q.Normalize();
                cube.BlockOrientation = new SerializableBlockOrientation(ref q);
            }

            // Rotate Groupings.
            foreach (var group in this.CubeGrid.BlockGroups)
            {
                for (var i = 0; i < group.Blocks.Count; i++)
                {
                    // The Group location is in the center of the cube.
                    // It doesn't have to be exact though, as it appears SE is just doing a location test of whatever object is at that location.
                    group.Blocks[i] = Vector3I.Transform(group.Blocks[i], quaternion);
                }
            }

            // TODO: Rotate ConveyorLines
            foreach (var conveyorLine in this.CubeGrid.ConveyorLines)
            {
                //conveyorLine.StartPosition = Vector3I.Transform(conveyorLine.StartPosition, quaternion);
                //conveyorLine.StartDirection = 
                //conveyorLine.EndPosition = Vector3I.Transform(conveyorLine.EndPosition, quaternion);
                //conveyorLine.EndDirection = 
            }

            // Rotate the ship also to maintain the appearance that it has not changed.
            var o = this.CubeGrid.PositionAndOrientation.Value.ToQuaternion() * VRageMath.Quaternion.Inverse(quaternion);
            o.Normalize();
            var p = new MyPositionAndOrientation(o.ToMatrix());

            this.CubeGrid.PositionAndOrientation = new MyPositionAndOrientation()
            {
                Position = this.CubeGrid.PositionAndOrientation.Value.Position,
                Forward = p.Forward,
                Up = p.Up
            };

            this.UpdateGeneralFromEntityBase();
        }

        public void ConvertToShip()
        {
            this.CubeGrid.IsStatic = false;
            this.UpdateGeneralFromEntityBase();
        }

        public bool ConvertToCornerArmor()
        {
            var count = 0;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeRoundArmor_Corner.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeBlockArmorCorner.ToString(); return c; }).ToList().Count;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeRoundArmor_Slope.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeBlockArmorSlope.ToString(); return c; }).ToList().Count;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeRoundArmor_CornerInv.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeBlockArmorCornerInv.ToString(); return c; }).ToList().Count;
            return count > 0;
        }

        public bool ConvertToRoundArmor()
        {
            var count = 0;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeBlockArmorCorner.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeRoundArmor_Corner.ToString(); return c; }).ToList().Count;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeBlockArmorSlope.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeRoundArmor_Slope.ToString(); return c; }).ToList().Count;
            count += this.CubeGrid.CubeBlocks.Where(c => c.SubtypeName == SubtypeId.LargeBlockArmorCornerInv.ToString()).Select(c => { c.SubtypeName = SubtypeId.LargeRoundArmor_CornerInv.ToString(); return c; }).ToList().Count;
            return count > 0;
        }

        public bool ConvertLadderToPassage()
        {
            var list = this.CubeGrid.CubeBlocks.Where(c => c is MyObjectBuilder_Ladder).ToArray();

            for (var i = 0; i < list.Length; i++)
            {
                var c = new MyObjectBuilder_Passage()
                {
                    EntityId = list[i].EntityId,
                    BlockOrientation = list[i].BlockOrientation,
                    BuildPercent = list[i].BuildPercent,
                    ColorMaskHSV = list[i].ColorMaskHSV,
                    IntegrityPercent = list[i].IntegrityPercent,
                    Min = list[i].Min,
                    SubtypeName = list[i].SubtypeName
                };
                this.CubeGrid.CubeBlocks.Remove(list[i]);
                this.CubeGrid.CubeBlocks.Add(c);
            }

            return list.Length > 0;
        }

        #region Mirror

        public bool MirrorModel(bool usePlane, bool oddMirror)
        {
            var xMirror = Mirror.None;
            var yMirror = Mirror.None;
            var zMirror = Mirror.None;
            var xAxis = 0;
            var yAxis = 0;
            var zAxis = 0;
            var count = 0;

            if (!usePlane)
            // Find mirror Axis.
            //if (!this.CubeGrid.XMirroxPlane.HasValue && !this.CubeGrid.YMirroxPlane.HasValue && !this.CubeGrid.ZMirroxPlane.HasValue)
            {
                // Find the largest contigious exterior surface to use as the mirror.
                var minX = this.CubeGrid.CubeBlocks.Min(c => c.Min.X);
                var maxX = this.CubeGrid.CubeBlocks.Max(c => c.Min.X);
                var minY = this.CubeGrid.CubeBlocks.Min(c => c.Min.Y);
                var maxY = this.CubeGrid.CubeBlocks.Max(c => c.Min.Y);
                var minZ = this.CubeGrid.CubeBlocks.Min(c => c.Min.Z);
                var maxZ = this.CubeGrid.CubeBlocks.Max(c => c.Min.Z);

                var countMinX = this.CubeGrid.CubeBlocks.Count(c => c.Min.X == minX);
                var countMinY = this.CubeGrid.CubeBlocks.Count(c => c.Min.Y == minY);
                var countMinZ = this.CubeGrid.CubeBlocks.Count(c => c.Min.Z == minZ);
                var countMaxX = this.CubeGrid.CubeBlocks.Count(c => c.Min.X == maxX);
                var countMaxY = this.CubeGrid.CubeBlocks.Count(c => c.Min.Y == maxY);
                var countMaxZ = this.CubeGrid.CubeBlocks.Count(c => c.Min.Z == maxZ);

                if (countMinX > countMinY && countMinX > countMinZ && countMinX > countMaxX && countMinX > countMaxY && countMinX > countMaxZ)
                {
                    xMirror = oddMirror ? Mirror.Odd : Mirror.EvenDown;
                    xAxis = minX;
                }
                else if (countMinY > countMinX && countMinY > countMinZ && countMinY > countMaxX && countMinY > countMaxY && countMinY > countMaxZ)
                {
                    yMirror = oddMirror ? Mirror.Odd : Mirror.EvenDown;
                    yAxis = minY;
                }
                else if (countMinZ > countMinX && countMinZ > countMinY && countMinZ > countMaxX && countMinZ > countMaxY && countMinZ > countMaxZ)
                {
                    zMirror = oddMirror ? Mirror.Odd : Mirror.EvenDown;
                    zAxis = minZ;
                }
                else if (countMaxX > countMinX && countMaxX > countMinY && countMaxX > countMinZ && countMaxX > countMaxY && countMaxX > countMaxZ)
                {
                    xMirror = oddMirror ? Mirror.Odd : Mirror.EvenUp;
                    xAxis = maxX;
                }
                else if (countMaxY > countMinX && countMaxY > countMinY && countMaxY > countMinZ && countMaxY > countMaxX && countMaxY > countMaxZ)
                {
                    yMirror = oddMirror ? Mirror.Odd : Mirror.EvenUp;
                    yAxis = maxY;
                }
                else if (countMaxZ > countMinX && countMaxZ > countMinY && countMaxZ > countMinZ && countMaxZ > countMaxX && countMaxZ > countMaxY)
                {
                    zMirror = oddMirror ? Mirror.Odd : Mirror.EvenUp;
                    zAxis = maxZ;
                }

                var cubes = MirrorCubes(this, false, xMirror, xAxis, yMirror, yAxis, zMirror, zAxis).ToArray();
                this.CubeGrid.CubeBlocks.AddRange(cubes);
                count += cubes.Length;
            }
            else
            {
                // Use the built in Mirror plane defined in game.
                if (this.CubeGrid.XMirroxPlane.HasValue)
                {
                    xMirror = this.CubeGrid.XMirroxOdd ? Mirror.EvenDown : Mirror.Odd; // Meaning is back to front? Or is it my reasoning?
                    xAxis = this.CubeGrid.XMirroxPlane.Value.X;
                    var cubes = MirrorCubes(this, true, xMirror, xAxis, Mirror.None, 0, Mirror.None, 0).ToArray();
                    this.CubeGrid.CubeBlocks.AddRange(cubes);
                    count += cubes.Length;

                    // TODO: mirror BlockGroups
                    // TODO: mirror ConveyorLines 
                }
                if (this.CubeGrid.YMirroxPlane.HasValue)
                {
                    yMirror = this.CubeGrid.YMirroxOdd ? Mirror.EvenDown : Mirror.Odd;
                    yAxis = this.CubeGrid.YMirroxPlane.Value.Y;
                    var cubes = MirrorCubes(this, true, Mirror.None, 0, yMirror, yAxis, Mirror.None, 0).ToArray();
                    this.CubeGrid.CubeBlocks.AddRange(cubes);
                    count += cubes.Length;

                    // TODO: mirror BlockGroups
                    // TODO: mirror ConveyorLines 
                }
                if (this.CubeGrid.ZMirroxPlane.HasValue)
                {
                    zMirror = this.CubeGrid.ZMirroxOdd ? Mirror.EvenUp : Mirror.Odd;
                    zAxis = this.CubeGrid.ZMirroxPlane.Value.Z;
                    var cubes = MirrorCubes(this, true, Mirror.None, 0, Mirror.None, 0, zMirror, zAxis).ToArray();
                    this.CubeGrid.CubeBlocks.AddRange(cubes);
                    count += cubes.Length;

                    // TODO: mirror BlockGroups
                    // TODO: mirror ConveyorLines 
                }
            }

            this.UpdateGeneralFromEntityBase();
            this.RaisePropertyChanged(() => BlockCount);
            return count > 0;
        }

        #region InvalidMirrorBlocks

        // TODO: As yet uncatered for blocks to Mirror.
        private static readonly SubtypeId[] InvalidMirrorBlocks = new SubtypeId[] {
            SubtypeId.Window1x2SideLeft,
            SubtypeId.Window1x2SideRight,
        };

        private static readonly string[] CornerRotationBlocks = new string[] {
            "LargeBlockArmorCorner",
            "LargeBlockArmorCornerRed",
            "LargeBlockArmorCornerYellow",
            "LargeBlockArmorCornerBlue",
            "LargeBlockArmorCornerGreen",
            "LargeBlockArmorCornerBlack",
            "LargeBlockArmorCornerWhite",
            "LargeBlockArmorCornerInv",
            "LargeBlockArmorCornerInvRed",
            "LargeBlockArmorCornerInvYellow",
            "LargeBlockArmorCornerInvBlue",
            "LargeBlockArmorCornerInvGreen",
            "LargeBlockArmorCornerInvBlack",
            "LargeBlockArmorCornerInvWhite",
            "LargeHeavyBlockArmorCorner",
            "LargeHeavyBlockArmorCornerRed",
            "LargeHeavyBlockArmorCornerYellow",
            "LargeHeavyBlockArmorCornerBlue",
            "LargeHeavyBlockArmorCornerGreen",
            "LargeHeavyBlockArmorCornerBlack",
            "LargeHeavyBlockArmorCornerWhite",
            "LargeHeavyBlockArmorCornerInv",
            "LargeHeavyBlockArmorCornerInvRed",
            "LargeHeavyBlockArmorCornerInvYellow",
            "LargeHeavyBlockArmorCornerInvBlue",
            "LargeHeavyBlockArmorCornerInvGreen",
            "LargeHeavyBlockArmorCornerInvBlack",
            "LargeHeavyBlockArmorCornerInvWhite",
            "SmallBlockArmorCorner",
            "SmallBlockArmorCornerRed",
            "SmallBlockArmorCornerYellow",
            "SmallBlockArmorCornerBlue",
            "SmallBlockArmorCornerGreen",
            "SmallBlockArmorCornerBlack",
            "SmallBlockArmorCornerWhite",
            "SmallBlockArmorCornerInv",
            "SmallBlockArmorCornerInvRed",
            "SmallBlockArmorCornerInvYellow",
            "SmallBlockArmorCornerInvBlue",
            "SmallBlockArmorCornerInvGreen",
            "SmallBlockArmorCornerInvBlack",
            "SmallBlockArmorCornerInvWhite",
            "SmallHeavyBlockArmorCorner",
            "SmallHeavyBlockArmorCornerRed",
            "SmallHeavyBlockArmorCornerYellow",
            "SmallHeavyBlockArmorCornerBlue",
            "SmallHeavyBlockArmorCornerGreen",
            "SmallHeavyBlockArmorCornerBlack",
            "SmallHeavyBlockArmorCornerWhite",
            "SmallHeavyBlockArmorCornerInv",
            "SmallHeavyBlockArmorCornerInvRed",
            "SmallHeavyBlockArmorCornerInvYellow",
            "SmallHeavyBlockArmorCornerInvBlue",
            "SmallHeavyBlockArmorCornerInvGreen",
            "SmallHeavyBlockArmorCornerInvBlack",
            "SmallHeavyBlockArmorCornerInvWhite",
            "LargeRoundArmor_Corner",
            "LargeRoundArmor_CornerRed",
            "LargeRoundArmor_CornerYellow",
            "LargeRoundArmor_CornerBlue",
            "LargeRoundArmor_CornerGreen",
            "LargeRoundArmor_CornerBlack",
            "LargeRoundArmor_CornerWhite",
            "LargeRoundArmor_CornerInv",
            "LargeRoundArmor_CornerInvRed",
            "LargeRoundArmor_CornerInvYellow",
            "LargeRoundArmor_CornerInvBlue",
            "LargeRoundArmor_CornerInvGreen",
            "LargeRoundArmor_CornerInvBlack",
            "LargeRoundArmor_CornerInvWhite",
        };

        internal static readonly string[] WindowFlatRotationBlocks = new string[] {
            "Window1x2Flat",
            "Window1x2FlatInv",
            "Window1x1Flat",
            "Window1x1FlatInv",
            "Window3x3Flat",
            "Window3x3FlatInv",
            "Window2x3Flat",
            "Window2x3FlatInv",
        };

        internal static readonly string[] WindowCornerRotationBlocks = new string[] {
            "Window1x1Face",
            "Window1x1Inv",
            "Window1x2Inv",
            "Window1x2Face",
        };

        internal static readonly string[] WindowEdgeRotationBlocks = new string[] {
            "Window1x1Side",
        };

        internal static readonly string[] TubeCurvedRotationBlocks = new string[] {
            "ConveyorTubeCurved",
            "ConveyorTubeCurvedMedium",
        };

        #endregion

        private static IEnumerable<MyObjectBuilder_CubeBlock> MirrorCubes(StructureCubeGridModel viewModel, bool integrate, Mirror xMirror, int xAxis, Mirror yMirror, int yAxis, Mirror zMirror, int zAxis)
        {
            var blocks = new List<MyObjectBuilder_CubeBlock>();
            SubtypeId outVal;

            if (xMirror == Mirror.None && yMirror == Mirror.None && zMirror == Mirror.None)
                return blocks;

            foreach (var block in viewModel.CubeGrid.CubeBlocks.Where(b => b.SubtypeName == "" || (Enum.TryParse<SubtypeId>(b.SubtypeName, out outVal) && !InvalidMirrorBlocks.Contains(outVal))))
            {
                var newBlock = block.Clone() as MyObjectBuilder_CubeBlock;
                newBlock.EntityId = block.EntityId == 0 ? 0 : SpaceEngineersApi.GenerateEntityId();

                if (block is MyObjectBuilder_MotorStator)
                {
                    ((MyObjectBuilder_MotorStator)newBlock).RotorEntityId = ((MyObjectBuilder_MotorStator)block).RotorEntityId == 0 ? 0 : SpaceEngineersApi.GenerateEntityId();
                }

                newBlock.BlockOrientation = MirrorCubeOrientation(block.SubtypeName, block.BlockOrientation, xMirror, yMirror, zMirror);
                var definition = SpaceEngineersApi.GetCubeDefinition(block.TypeId, viewModel.GridSize, block.SubtypeName);

                if (definition.Size.X == 1 && definition.Size.Y == 1 && definition.Size.z == 1)
                {
                    newBlock.Min = block.Min.Mirror(xMirror, xAxis, yMirror, yAxis, zMirror, zAxis);
                }
                else
                {
                    // resolve size of component, and transform to original orientation.
                    var orientSize = definition.Size.Add(-1).Transform(block.BlockOrientation).Abs();

                    var min = block.Min.Mirror(xMirror, xAxis, yMirror, yAxis, zMirror, zAxis);
                    var blockMax = new SerializableVector3I(block.Min.X + orientSize.X, block.Min.Y + orientSize.Y, block.Min.Z + orientSize.Z);
                    var max = blockMax.Mirror(xMirror, xAxis, yMirror, yAxis, zMirror, zAxis);

                    if (xMirror != Mirror.None)
                        newBlock.Min = new SerializableVector3I(max.X, min.Y, min.Z);
                    if (yMirror != Mirror.None)
                        newBlock.Min = new SerializableVector3I(min.X, max.Y, min.Z);
                    if (zMirror != Mirror.None)
                        newBlock.Min = new SerializableVector3I(min.X, min.Y, max.Z);
                }

                // Don't place a block if one already exists there in the mirror.
                if (integrate && viewModel.CubeGrid.CubeBlocks.Any(b => b.Min.X == newBlock.Min.X && b.Min.Y == newBlock.Min.Y && b.Min.Z == newBlock.Min.Z /*|| b.Max == newBlock.Min*/))  // TODO: check cubeblock size.
                    continue;

                blocks.Add(newBlock);
            }
            return blocks;
        }

        private static readonly Dictionary<OrientType, SerializableBlockOrientation> BaseOrientations = new Dictionary<OrientType, SerializableBlockOrientation>()
        {
            {OrientType.Axis24_Backward_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Down)},
            {OrientType.Axis24_Backward_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Left)},
            {OrientType.Axis24_Backward_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Right)},
            {OrientType.Axis24_Backward_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Up)},
            {OrientType.Axis24_Down_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Backward)},
            {OrientType.Axis24_Down_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Forward)},
            {OrientType.Axis24_Down_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Left)},
            {OrientType.Axis24_Down_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Right)},
            {OrientType.Axis24_Forward_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Down)},
            {OrientType.Axis24_Forward_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Left)},
            {OrientType.Axis24_Forward_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Right)},
            {OrientType.Axis24_Forward_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)},
            {OrientType.Axis24_Left_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Backward)},
            {OrientType.Axis24_Left_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Down)},
            {OrientType.Axis24_Left_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Forward)},
            {OrientType.Axis24_Left_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Up)},
            {OrientType.Axis24_Right_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Backward)},
            {OrientType.Axis24_Right_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Down)},
            {OrientType.Axis24_Right_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Forward)},
            {OrientType.Axis24_Right_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Up)},
            {OrientType.Axis24_Up_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Backward)},
            {OrientType.Axis24_Up_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Forward)},
            {OrientType.Axis24_Up_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Left)},
            {OrientType.Axis24_Up_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Right)},
        };

        private static SerializableBlockOrientation MirrorCubeOrientation(string subtypeName, SerializableBlockOrientation orientation, Mirror xMirror, Mirror yMirror, Mirror zMirror)
        {
            if (xMirror != Mirror.None)
            {
                #region X Symmetry mapping

                if (CornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Right];
                    }
                }
                else if (WindowFlatRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Left];
                    }
                }
                else if (WindowCornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Forward];
                    }
                }
                else if (WindowEdgeRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Left_Up];
                    }
                }
                else if (TubeCurvedRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Left];
                        default: return orientation;
                    }
                }
                else
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Left];
                    }
                }

                #endregion
            }
            else if (yMirror != Mirror.None)
            {
                #region Y Symmetry mapping

                if (CornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Up_Backward];
                    }
                }
                else if (WindowFlatRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Left];
                    }
                }
                else if (WindowCornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Forward];
                    }
                }
                else if (WindowEdgeRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Right_Down];
                    }
                }
                else if (TubeCurvedRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Down];
                        default: return orientation;
                    }
                }
                else
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                    }
                }

                #endregion
            }
            else if (zMirror != Mirror.None)
            {
                #region Z Symmetry mapping

                if (CornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Down_Forward];
                    }
                }
                else if (WindowFlatRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Up]; //U
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Left];
                    }
                }
                else if (WindowCornerRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Backward];
                    }
                }
                else if (WindowEdgeRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Right_Up];
                    }
                }
                else if (TubeCurvedRotationBlocks.Contains(subtypeName))
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                        default: return orientation;
                    }
                }
                else
                {
                    var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                    switch (cubeType.Key)
                    {
                        case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Forward_Down];
                        case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Forward_Left];
                        case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                        case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Forward_Up];
                        case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                        case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                        case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Left];
                        case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                        case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Backward_Down];
                        case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Backward_Left];
                        case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                        case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Backward_Up];
                        case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                        case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Down];
                        case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                        case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Up];
                        case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                        case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Down];
                        case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                        case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Up];
                        case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                        case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                        case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Left];
                        case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                    }
                }

                //var cubeType = BaseOrientations.FirstOrDefault(x => x.Value.Forward == orientation.Forward && x.Value.Up == orientation.Up);
                //switch (cubeType.Key)
                //{
                //    case OrientType.Axis24_Backward_Down: return BaseOrientations[OrientType.Axis24_Backward_Down];
                //    case OrientType.Axis24_Backward_Left: return BaseOrientations[OrientType.Axis24_Backward_Left];
                //    case OrientType.Axis24_Backward_Right: return BaseOrientations[OrientType.Axis24_Backward_Right];
                //    case OrientType.Axis24_Backward_Up: return BaseOrientations[OrientType.Axis24_Backward_Up];
                //    case OrientType.Axis24_Down_Backward: return BaseOrientations[OrientType.Axis24_Down_Backward];
                //    case OrientType.Axis24_Down_Forward: return BaseOrientations[OrientType.Axis24_Down_Forward];
                //    case OrientType.Axis24_Down_Left: return BaseOrientations[OrientType.Axis24_Down_Left];
                //    case OrientType.Axis24_Down_Right: return BaseOrientations[OrientType.Axis24_Down_Right];
                //    case OrientType.Axis24_Forward_Down: return BaseOrientations[OrientType.Axis24_Forward_Down];
                //    case OrientType.Axis24_Forward_Left: return BaseOrientations[OrientType.Axis24_Forward_Left];
                //    case OrientType.Axis24_Forward_Right: return BaseOrientations[OrientType.Axis24_Forward_Right];
                //    case OrientType.Axis24_Forward_Up: return BaseOrientations[OrientType.Axis24_Forward_Up];
                //    case OrientType.Axis24_Left_Backward: return BaseOrientations[OrientType.Axis24_Left_Backward];
                //    case OrientType.Axis24_Left_Down: return BaseOrientations[OrientType.Axis24_Left_Down];
                //    case OrientType.Axis24_Left_Forward: return BaseOrientations[OrientType.Axis24_Left_Forward];
                //    case OrientType.Axis24_Left_Up: return BaseOrientations[OrientType.Axis24_Left_Up];
                //    case OrientType.Axis24_Right_Backward: return BaseOrientations[OrientType.Axis24_Right_Backward];
                //    case OrientType.Axis24_Right_Down: return BaseOrientations[OrientType.Axis24_Right_Down];
                //    case OrientType.Axis24_Right_Forward: return BaseOrientations[OrientType.Axis24_Right_Forward];
                //    case OrientType.Axis24_Right_Up: return BaseOrientations[OrientType.Axis24_Right_Up];
                //    case OrientType.Axis24_Up_Backward: return BaseOrientations[OrientType.Axis24_Up_Backward];
                //    case OrientType.Axis24_Up_Forward: return BaseOrientations[OrientType.Axis24_Up_Forward];
                //    case OrientType.Axis24_Up_Left: return BaseOrientations[OrientType.Axis24_Up_Left];
                //    case OrientType.Axis24_Up_Right: return BaseOrientations[OrientType.Axis24_Up_Right];
                //}

                #endregion
            }

            return orientation;
        }

        #endregion

        private static string GetAxisIndicator(Base6Directions.Direction direction)
        {
            switch (Base6Directions.GetAxis(direction))
            {
                case Base6Directions.Axis.LeftRight: // X
                    if (Base6Directions.GetVector(direction).X < 0)
                        return "-X";
                    else
                        return "+X";
                case Base6Directions.Axis.UpDown: // Y
                    if (Base6Directions.GetVector(direction).Y < 0)
                        return "-Y";
                    else
                        return "+Y";
                case Base6Directions.Axis.ForwardBackward: // Z
                    if (Base6Directions.GetVector(direction).Z < 0)
                        return "-Z";
                    else
                        return "+Z";
            }

            return null;
        }

        #endregion
    }
}
