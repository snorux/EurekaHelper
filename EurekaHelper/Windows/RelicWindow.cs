using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace EurekaHelper.Windows
{
    internal class RelicWindow : Window, IDisposable
    {
        private readonly EurekaHelper _plugin = null!;

        public RelicWindow(EurekaHelper plugin) : base("Eureka Helper - Relic")
        {
            _plugin = plugin;
            SizeConstraints = new WindowSizeConstraints { MaximumSize = new Vector2(float.MaxValue, float.MaxValue) };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("ERelicTab"))
            {
                if (ImGui.BeginTabItem("Anemos"))
                {
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pagos"))
                {
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pyros"))
                {
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Hydatos"))
                {
                    ImGui.EndTabItem();
                }
            }
        }
    }
}
