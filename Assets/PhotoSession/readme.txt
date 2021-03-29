Scripting Define Symbols
-------------------------------
+ for HDRP use: USING_HDRP
+ for URP use: USING_URP

Known Issues
-------------------------------
* bright edges with R.A.M. rivers in HDRP
  looks like a bug in combination with Camera.Render(), you can briefly see the flicker even when you start play mode.
  cause: Exposure set to Automatic
  workaround: set Exposure to Fixed
