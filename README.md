# unity_image_with_hit_detection_on_shape
Have you ever wanted a button whose click-area tightly hugs the shape of its image?
One solution is to turn on "Writable" on the image's texture and so an alpha hit-test. That's pretty costly however if you have a lot of those kind of buttons (it greatly increases the size of the texture both in the bundle and in memory)
With this triangle based hit-detector, we can now turn off "writable" on those big textures. This is a big win for App size and memory footprint.

Usage:
- Go to the texture's properties and use the Shape Editor do draw the clickable area.
- On the Button, remove the Image component.
- Add an ImageWithHitDetectionOnShape component.
- Don't forget to patch up the &lt;Missing Image> link the in the Button component again.
