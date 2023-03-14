namespace BindablePropertyTest;

using BindablePropertyAttributes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Input;

public partial class BottomDrawer : ContentView
{
    //  Backing store
    //
    readonly Frame      _bottomDrawerFrame;
    readonly Grid       _bottomDrawerGrid;

    //  Bound properties
    //
    [BindableProperty( DefaultValue = "Colors.White", HidesBaseProperty = "true")]
    public Color    _backgroundColor;

    [BindableProperty( DefaultValue = @"Color.FromArgb(""#4B000000"")")]
    public Color    _backDropColor;

    [BindableProperty( DefaultValue = "300.0", HidesBaseProperty = "true" )]
    public double   _heightRequest;

    [BindableProperty( DefaultValue = "-1.0", HidesBaseProperty = "true" )]
    public double   _widthRequest;

    [BindableProperty( DefaultValue = "0.0" )]
    public double   _adjustment;

    [BindableProperty( DefaultValue = "250" )]
    public int      _duration;

    [BindableProperty( DefaultValue = "false", PropertyChanged = "OnOpenClose" )]
    public bool     _isOpen;

    [BindableProperty( DefaultValue = "null"  )]
    public IRelayCommand  _statusChangedCommand;

    [BindableProperty( DefaultValue = "false"  )]
    public bool     _statusChangedCommandParameter;

    readonly WeakEventManager   _eventManager   = new();

    public delegate void StatusChangedEventHandler( object sender, BottomDrawerStateChangedEventArgs e );

    public event EventHandler<BottomDrawerStateChangedEventArgs> OnStatusChanged
    {
        add     =>   _eventManager.AddEventHandler( value );
        remove  =>   _eventManager.RemoveEventHandler( value );
    }

    public BottomDrawer()
    {
        InitializeComponent();

        _bottomDrawerGrid   = (Grid)Children[ 0 ];
        _bottomDrawerFrame  = _bottomDrawerGrid.FindByName<Frame>( "bottomToolbar" );

        //  Initially closed
        //
        _bottomDrawerFrame.TranslationY = Adjustment + HeightRequest;
        IsOpen = false;
        InputTransparent = true;

        _ = Task.Run( async () => await CloseDrawerAsync( true ).ConfigureAwait( false ) );
    }

    /// <summary>
    /// Open is been set by the user
    /// </summary>
    static void OnOpenClose( BindableObject bindable, object oldValue, object newValue )
    {
        if ( bindable is BottomDrawer drawer && oldValue is bool ov && newValue is bool nv )
        {
            if ( ov == nv )
                return;

            _ = nv
                ? Task.Run( drawer.OpenDrawerAsync )
                : Task.Run( async () => await drawer.CloseDrawerAsync().ConfigureAwait( true ) );
        }
    }

    async Task OpenDrawerAsync()
            => await MainThread.InvokeOnMainThreadAsync( async () =>
               {
                   _ = await _bottomDrawerFrame.TranslateTo( 0.0, 0.0, (uint)Duration, Easing.SinIn ).ConfigureAwait( true );

                   InputTransparent = false;

                   _eventManager.HandleEvent( this, new BottomDrawerStateChangedEventArgs( open: true ), nameof( OnStatusChanged ) );

                   if ( StatusChangedCommand?.CanExecute( true ) is true )
                       StatusChangedCommand.Execute( true );

               } ).ConfigureAwait( true );

    async Task CloseDrawerAsync( bool firstTime = false )
            => await MainThread.InvokeOnMainThreadAsync( async () =>
               {
                   _ = await _bottomDrawerFrame.TranslateTo( 0.0, Adjustment + HeightRequest, (uint)Duration, Easing.SinIn ).ConfigureAwait( true );

                   InputTransparent = true;

                   if ( firstTime )
                       return;

                   _eventManager.HandleEvent( this, new BottomDrawerStateChangedEventArgs( open: false ), nameof( OnStatusChanged ) );

                   if ( StatusChangedCommand?.CanExecute( false ) is true )
                       StatusChangedCommand.Execute( false );

               } ).ConfigureAwait( true );
}

public class BottomDrawerStateChangedEventArgs : EventArgs
{
    public bool IsOpen { get; set; }
    public BottomDrawerStateChangedEventArgs( bool open ) => IsOpen = open;
}
