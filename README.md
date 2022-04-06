# unity_image_with_hit_detection_on_shape
Have you ever wanted a button whose click-area tightly hugs the shape of its image?
One solution is to turn on "Writable" on the image's texture and do an alpha hit-test. That's pretty costly however if you have a lot of those kind of buttons (it greatly increases the size of the texture both in the bundle and in memory).

With this triangle based hit-detector, we can now turn off "writable" on those big textures. This is a big win for App size and memory footprint.

Usage:
- On the Button, remove the Image component.
- Add an ImageWithHitDetectionOnShape component.
- Don't forget to patch up the &lt;Missing Image> link the in the Button component again.
- Check "Use Sprite Shape" on the image if you haven't already.

The button will use Unity's sprite shape by default. If that's not fitting very well, or you want a custom one (i.e. no holes in the middle, padding):
- Go to the texture's properties and use the Shape Editor do draw the clickable area.

Because this component uses hit-testing with triangles on the button, you could extend it with functionality to test other shapes as well.

Features:
- Matches Image on height, scale, rotation, free-form width & height
- Matches Image when "Preserve Aspect Ratio" is checked
- Matches Image when "Simple" or "Filled" is selected
- Commented out debug code to visually see how (and if) it works

Limitations:
- Doesn't match exactly when image type "Sliced" is selected
- Doesn't match when image type "Repeat" is selected

Future:
- If someone can tell me how to get the triangle mesh for an image that has type "Slice", that would be great.
