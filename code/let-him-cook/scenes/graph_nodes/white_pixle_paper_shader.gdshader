shader_type canvas_item;

uniform sampler2D paperTexture;
uniform vec3 target_color = vec3(0.772, 0.769, 0.769); // #c5c4c4 in 0–1 Werten
uniform float color_threshold = 0.05; // Wie tolerant der Vergleich ist (höher = mehr Toleranz)

void fragment() {
    vec4 base_color = texture(TEXTURE, UV);
    vec3 diff = abs(base_color.rgb - target_color);

    // Berechne, wie "weit" der Pixel von der Ziel-Farbe entfernt ist
    float distance = max(max(diff.r, diff.g), diff.b);

    if (distance < color_threshold) {
        // Wenn der Pixel ungefähr "weiß/grau" ist → nimm die zweite Textur
        COLOR = texture(paperTexture, SCREEN_UV * vec2(0.8, 1.0));
    } else {
        // Sonst: Originalfarbe behalten
        COLOR = base_color;
    }
}