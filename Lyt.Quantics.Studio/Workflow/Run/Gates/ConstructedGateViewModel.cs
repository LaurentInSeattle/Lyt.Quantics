namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class ConstructedGateViewModel : Bindable<ConstructedGateView>
{
    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }
}


/*
 *
 
 Swap X layout 
		<Rectangle
			Grid.Row="0" Grid.RowSpan="2"
			Height="16" Width="2"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Fill="{StaticResource ResourceKey= LightAqua_2_100}"
			>
			<Rectangle.RenderTransform>
				<RotateTransform Angle="45" />
			</Rectangle.RenderTransform>
		</Rectangle>
		<Rectangle
			Grid.Row="0" Grid.RowSpan="2"
			Height="2" Width="16"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Fill="{StaticResource ResourceKey= LightAqua_2_100}"
			>
			<Rectangle.RenderTransform>
				<RotateTransform Angle="45" />
			</Rectangle.RenderTransform>
		</Rectangle>

 
 Control Ellipse 

		<Ellipse
			Grid.Row="3" Grid.RowSpan="2"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Height="12" Width="12"
			Fill="{StaticResource ResourceKey= LightAqua_2_100}"
			/>

 
 ANTI-Control Ellipse 

		<Ellipse
			Grid.Row="0" Grid.RowSpan="2"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Height="12" Width="12"
			Fill="AntiqueWhite"		
			Stroke="{StaticResource ResourceKey= LightAqua_2_100}"		
			StrokeThickness="2"
			/>


 NOT Ellipse with + 

		<Ellipse
			Grid.Row="0" Grid.RowSpan="2"
			Height="16" Width="16"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Stroke="{StaticResource ResourceKey= LightAqua_2_100}"
			StrokeThickness="1"
			Fill="Transparent"
			/>
		<Rectangle
			Grid.Row="0" Grid.RowSpan="2"
			Height="14" Width="1"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Fill="{StaticResource ResourceKey= LightAqua_2_100}"
			/>
		<Rectangle
			Grid.Row="0" Grid.RowSpan="2"
			Height="1" Width="14"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Fill="{StaticResource ResourceKey= LightAqua_2_100}"
			/>

 * 
 */