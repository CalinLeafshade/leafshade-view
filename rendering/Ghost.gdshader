shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

// Params = (wave frequency in Hz, number of waves per unit distance)
//
const vec2 params = vec2(0.5, 7.0);
	
// Simple circular wave function
float wave(vec2 pos, float t, float freq, float numWaves, vec2 center) {
	float d = length(pos - center);
	d = log(1.0 + exp(d));
	return 1.0/(1.0+20.0*d*d) *
		   sin(2.0*3.1415*(-numWaves*d + t*freq));
}

// This height map combines a couple of waves
float height(vec2 pos, float t) {
	float w;
	w =  wave(pos, t, params.x, params.y, vec2(0.5, -0.5));
	w += wave(pos, t, params.x, params.y, -vec2(0.5, -0.5));
	return w * 0.1;
}

// Discrete differentiation
vec2 normal(vec2 pos, float t) {
	return 	vec2(height(pos - vec2(0.01, 0), t) - height(pos, t), 
				 height(pos - vec2(0, 0.01), t) - height(pos, t));
}


void fragment() {
	float w = SCREEN_PIXEL_SIZE.x * 5.0;
	float h = SCREEN_PIXEL_SIZE.y * 5.0;
	vec4 n[9];
	
	vec2 coord = SCREEN_UV;

	n[0] = texture(screen_texture, coord + vec2( -w, -h));
	n[1] = texture(screen_texture, coord + vec2(0.0, -h));
	n[2] = texture(screen_texture, coord + vec2(  w, -h));
	n[3] = texture(screen_texture, coord + vec2( -w, 0.0));
	n[4] = texture(screen_texture, coord);
	n[5] = texture(screen_texture, coord + vec2(  w, 0.0));
	n[6] = texture(screen_texture, coord + vec2( -w, h));
	n[7] = texture(screen_texture, coord + vec2(0.0, h));
	n[8] = texture(screen_texture, coord + vec2(  w, h));
	
	vec4 sobel_edge_h = n[2] + (2.0*n[5]) + n[8] - (n[0] + (2.0*n[3]) + n[6]);
  	vec4 sobel_edge_v = n[0] + (2.0*n[1]) + n[2] - (n[6] + (2.0*n[7]) + n[8]);
	vec4 sobel = sqrt((sobel_edge_h * sobel_edge_h) + (sobel_edge_v * sobel_edge_v));

	
	
	vec2 uvn = 2.0 * SCREEN_UV - vec2(1.0);
	vec2 uv = SCREEN_UV + normal(uvn, TIME);
	vec4 ripple = texture(screen_texture, vec2(uv.x, uv.y));
	vec4 col = texture(screen_texture, SCREEN_UV);
	
	//COLOR = col + ripple * sobel.a * 0.4;
	float glow = (sin(TIME) + 1.0) / 2.0;
	COLOR *= mix(col, ripple, (sobel.a * 0.8 + 0.1)) + ripple * glow * 0.3;
	//COLOR = sobel;
	//COLOR.r = sobel.a; // = vec4( 1.0 - sobel.rgb, 1.0 );
	
}