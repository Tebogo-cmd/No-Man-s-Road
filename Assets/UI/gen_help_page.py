"""
Generates HelpPage.png - a torn-page style game manual for No Man's Road.
Run once: python gen_help_page.py

SCALE controls render resolution. SCALE=2 doubles every dimension so
text stays sharp when Unity displays the image at large sizes.
"""
from PIL import Image, ImageDraw, ImageFont
import random, os

random.seed(7)

SCALE = 2   # change to 1 for a smaller file, 3 for ultra-high-res

def s(v):
    """Scale a pixel value."""
    return int(v * SCALE)

# ── Canvas ────────────────────────────────────────────────────────────────────
W, H = s(1200), s(1750)
OUT  = os.path.join(os.path.dirname(__file__), "HelpPage.png")

# ── Palette ───────────────────────────────────────────────────────────────────
PAPER       = (250, 238, 210)
PAPER_MID   = (238, 222, 190)
PAPER_DARK  = (210, 192, 160)
TORN_FIBER  = (180, 145, 100)
INK         = (28,  18,  8)
HEADER_INK  = (110, 42,  8)
TITLE_INK   = (60,  25,  5)
RULE_RED    = (195, 80,  70)
RULE_BLUE   = (100, 130, 185)
DIVIDER     = (160, 128, 90)

# ── Fonts ─────────────────────────────────────────────────────────────────────
FD = "C:/Windows/Fonts/"
F_TITLE_B = ImageFont.truetype(FD + "georgiab.ttf", s(68))
F_SUBTITLE = ImageFont.truetype(FD + "georgiai.ttf", s(30))
F_HEAD     = ImageFont.truetype(FD + "georgiab.ttf", s(34))
F_BODY     = ImageFont.truetype(FD + "georgia.ttf",  s(27))
F_BODY_B   = ImageFont.truetype(FD + "georgiab.ttf", s(27))
F_SMALL    = ImageFont.truetype(FD + "georgiai.ttf", s(22))

# ── Paper bounds ──────────────────────────────────────────────────────────────
PL = s(75)
PR = W - s(75)
PT = s(115)
PB = H - s(115)

# ── Torn-edge helper ──────────────────────────────────────────────────────────
def torn_pts(x0, x1, y_nom, amp=s(22), n=90):
    pts = []
    step = (x1 - x0) / n
    y = y_nom
    for i in range(n + 1):
        x = x0 + i * step
        y += random.uniform(-s(3), s(3))
        y  = y_nom + max(-amp * 2, min(amp * 2, y - y_nom))
        spike = amp * 2.2 if random.random() < 0.07 else 0
        pts.append((x, y + random.uniform(-amp, amp) + random.choice([-1, 1]) * spike))
    return pts

top_edge = torn_pts(PL, PR, PT, amp=s(24))
bot_edge = torn_pts(PL, PR, PB, amp=s(24))

paper_poly = (top_edge +
              [(PR, PB + s(30)), (PR, PB - s(30))] +
              list(reversed(bot_edge)) +
              [(PL, PT + s(30)), (PL, PT - s(30))])

# ── Build image ───────────────────────────────────────────────────────────────
img  = Image.new("RGBA", (W, H), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Semi-transparent drop shadow
sh = [(x + s(9), y + s(9)) for x, y in paper_poly]
draw.polygon(sh, fill=(0, 0, 0, 120))

# Paper body
draw.polygon(paper_poly, fill=PAPER)

# Subtle horizontal rule lines
for y in range(PT + s(55), PB - s(30), s(32)):
    draw.line([(PL + s(15), y), (PR - s(15), y)], fill=PAPER_MID, width=1)

# Left red margin line
MX = PL + s(88)
draw.line([(MX, PT + s(45)), (MX, PB - s(45))], fill=RULE_RED, width=s(2))

# Torn-edge fibers
def draw_fibers(edge_pts, direction):
    for i in range(0, len(edge_pts) - 1, 2):
        x, y = edge_pts[i]
        length = random.randint(s(4), s(18))
        x2 = x + random.uniform(-s(3), s(3))
        y2 = y + direction * length
        draw.line([(x, y), (x2, y2)], fill=TORN_FIBER, width=1)

draw_fibers(top_edge, -1)
draw_fibers(bot_edge,  1)

# Torn-edge highlight
for pts, dy in [(top_edge, +s(6)), (bot_edge, -s(6))]:
    shifted = [(x, y + dy) for x, y in pts]
    for i in range(len(shifted) - 1):
        draw.line([shifted[i], shifted[i + 1]], fill=PAPER_DARK, width=s(3))

# ── Layout helpers ────────────────────────────────────────────────────────────
CX = (PL + PR) // 2
TX = MX + s(28)       # text left margin
RX = PR - s(30)       # text right margin
y  = PT + s(58)

def centre_text(text, font, colour, y_pos):
    bb = draw.textbbox((0, 0), text, font=font)
    tw = bb[2] - bb[0]
    draw.text((CX - tw // 2, y_pos), text, font=font, fill=colour)
    return y_pos + (bb[3] - bb[1]) + s(6)

def left_text(text, font, colour, y_pos, indent=0):
    draw.text((TX + indent, y_pos), text, font=font, fill=colour)
    bb = draw.textbbox((0, 0), text, font=font)
    return y_pos + (bb[3] - bb[1]) + s(10)

def divider(y_pos, gap=s(18)):
    y_pos += gap // 2
    draw.line([(TX, y_pos), (RX, y_pos)], fill=DIVIDER, width=1)
    return y_pos + gap // 2

def section_header(text, y_pos):
    y_pos += s(14)
    draw.text((TX, y_pos), text, font=F_HEAD, fill=HEADER_INK)
    bb = draw.textbbox((0, 0), text, font=F_HEAD)
    y_pos += (bb[3] - bb[1]) + s(12)   # gap between text and underline
    draw.line([(TX, y_pos), (TX + (bb[2] - bb[0]) + s(10), y_pos)],
              fill=HEADER_INK, width=s(2))
    return y_pos + s(12)

def key_val(key, val, y_pos, gap=s(30)):
    """Bold key, then regular value, always gap pixels apart."""
    draw.text((TX + s(16), y_pos), key, font=F_BODY_B, fill=INK)
    kbb = draw.textbbox((0, 0), key, font=F_BODY_B)
    kw  = kbb[2] - kbb[0]
    draw.text((TX + s(16) + kw + gap, y_pos), val, font=F_BODY, fill=INK)
    return y_pos + (kbb[3] - kbb[1]) + s(14)

# ── TITLE BLOCK ───────────────────────────────────────────────────────────────
for i, thick in enumerate([1, 2]):
    lx = CX - s(280) + i * s(5)
    rx = CX + s(280) - i * s(5)
    draw.line([(lx, y + i * s(5)), (rx, y + i * s(5))], fill=RULE_BLUE, width=thick)
y += s(16)

y = centre_text("NO MAN'S ROAD",  F_TITLE_B, TITLE_INK, y)
y = centre_text("Player's Manual", F_SUBTITLE, HEADER_INK, y)

for i, thick in enumerate([2, 1]):
    lx = CX - s(280) + i * s(5)
    rx = CX + s(280) - i * s(5)
    draw.line([(lx, y + i * s(5)), (rx, y + i * s(5))], fill=RULE_BLUE, width=thick)
y += s(22)

# ── HOW TO PLAY ───────────────────────────────────────────────────────────────
y = section_header("HOW TO PLAY", y)
y = key_val("Left Arrow",  "Switch Lane Left",  y)
y = key_val("Right Arrow", "Switch Lane Right", y)
y = key_val("Space",       "Jump",              y)
y = left_text("Run through 3 lanes, dodge obstacles, collect", F_BODY, INK, y, indent=s(16))
y = left_text("items, and survive as long as possible!",       F_BODY, INK, y, indent=s(16))

y = divider(y)

# ── SCORING ───────────────────────────────────────────────────────────────────
y = section_header("SCORING", y)
y = key_val("Egg  (common)", "1 pt  x  Multiplier",  y)
y = key_val("Gem  (rare)",   "10 pts  x  Multiplier", y)
y = left_text("Your score is shown live at the top of the screen.", F_BODY, INK, y, indent=s(16))

y = divider(y)

# ── HAZARDS ───────────────────────────────────────────────────────────────────
y = section_header("HAZARDS", y)
y = key_val("Car  -  front hit", "-50  health", y)
y = key_val("Car  -  side hit",  "-15  health", y)
y = key_val("Dog",               "-20  health", y)
y = key_val("Tank Bomb",         "-30  health", y)
y = left_text("Jump over cars or switch lanes to avoid damage.", F_BODY, INK, y, indent=s(16))

y = divider(y)

# ── POWER-UPS ─────────────────────────────────────────────────────────────────
y = section_header("POWER-UPS", y)
y = key_val("[*]  Score Multiplier", "Lasts 20 seconds", y)
y = left_text("Pick up the multiplier icon to stack a +1 bonus",     F_BODY, INK, y, indent=s(32))
y = left_text("onto every future score. Stacks with more pickups!",  F_BODY, INK, y, indent=s(32))

y = divider(y)

# ── GENERAL INFO ──────────────────────────────────────────────────────────────
y = section_header("GENERAL INFO", y)
y = left_text("- You start with 100 health points.",             F_BODY, INK, y, indent=s(16))
y = left_text("- Game ends when health reaches zero.",            F_BODY, INK, y, indent=s(16))
y = left_text("- Health is shown as a slider on-screen.",        F_BODY, INK, y, indent=s(16))
y = left_text("- Use Restart or Quit on the Game Over screen.",  F_BODY, INK, y, indent=s(16))
y = left_text("- The road speeds up as obstacles increase.",     F_BODY, INK, y, indent=s(16))

y = divider(y, gap=s(24))

y = centre_text("Version 1.0  -  No Man's Road  -  FinalGameC", F_SMALL, (140, 110, 70), y + s(10))

# Coffee-stain ring detail
for r in [s(55), s(52), s(49)]:
    cx2, cy2 = PR - s(110), PB - s(120)
    draw.ellipse([(cx2 - r, cy2 - r), (cx2 + r, cy2 + r)],
                 outline=(160, 120, 70), width=s(2))

# ── Save ──────────────────────────────────────────────────────────────────────
img.save(OUT)
print(f"Saved -> {OUT}  ({W}x{H} px, SCALE={SCALE})")
