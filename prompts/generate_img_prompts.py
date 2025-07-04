resource_types = [
    "coal",
    "oil",
    "coal and oil",
    "garbage",
    "uranium",
    "wind or solar",
    "garbage and oil"
]

prompt_template = (
    "An isometric illustration of a stylized industrial power plant, designed for a modern strategy board game. "
    "The plant uses {resource} as its primary energy source, reflected through visual cues "
    "(e.g., smokestacks for coal, oil tanks, garbage incinerators, cooling towers for uranium, "
    "solar panels, or wind turbines). Complexity level {level} (on a scale from 1 to 10): "
    "level 1 plants are small, simple, and old-fashioned; level 10 plants are massive, intricate, and futuristic "
    "with advanced machinery. The art style is semi-realistic and industrial. "
    "Strong shading and clear isometric perspective. "
    "No people, no icons, no text, no background—only the power plant."
)

# Generate prompts
for resource in resource_types:
    for level in range(1, 11):
        prompt = prompt_template.format(resource=resource, level=level)
        print(f"---\n# {resource.title()} - Complexity {level}\n{prompt}\n")
