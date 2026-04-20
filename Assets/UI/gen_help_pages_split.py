"""
Generates two high-resolution help page PNGs for No Man's Road:
  HelpPage_1.png  –  Title / How to Play / Scoring / Hazards
  HelpPage_2.png  –  Power-Ups / General Info / Footer

Splitting into two images halves the height of each, so Unity can
display them at a larger native size without blurring the text.

Run once: python gen_help_pages_split.py
"""
from PIL import Image, ImageDraw, ImageFont
import random, os

SCALE = 2   # increase for even sharper text (3 = ultra-high-res)

def s(v):
    return int(v * SCALE)

# ── Shared palette ────────────────────────────────────────────────────────────
PAPER      = (250, 238, 210)
PAPER_MID  = (238, 222, 190)
PAPER_DARK = (210, 192, 160)
TORN_FIBER = (180, 145, 100)
INK        = (28,  18,  8)
HEADER_INK = (110, 42,  8)
TITLE_INK  = (60,  25,  5)
RULE_RED   = (195, 80,  70)
RULE_BLUE  = (100, 130, 185)
DIVIDER    = (160, 128, 90)

# ── Shared fonts ──────────────────────────────────────────────────────────────
FD         = "C:/Windows/Fonts/"
F_TITLE_B  = ImageFont.truetype(FD + "georgiab.ttf", s(68))
F_SUBTITLE = ImageFont.truetype(FD + "georgiai.ttf", s(30))
F_HEAD     = ImageFont.truetype(FD + "georgiab.ttf", s(34))
F_BODY     = ImageFont.truetype(FD + "georgia.ttf",  s(27))
F_BODY_B   = ImageFont.truetype(FD + "georgiab.ttf", s(27))
F_SMALL    = ImageFont.truetype(FD + "georgiai.ttf", s(22))

DIR = os.path.dirname(__file__)


# ─────────────────────────────────────────────────────────────────────────────
#  Page builder
# ─────────────────────────────────────────────────────────────────────────────
def build_page(seed, W, H, content_fn, out_path):
    """
    Draw a torn-paper page of size W×H, call content_fn(draw, layout)
    to fill it, then save to out_path.
    """
    random.seed(seed)

    PL = s(75)
    PR = W - s(75)
    PT = s(100)
    PB = H - s(100)

    # ── Torn edges ───────────────────────────────────────────────────────────
    def torn_pts(x0, x1, y_nom, amp=s(22), n=90):
        pts, step, y = [], (x1 - x0) / n, y_nom
        for i in range(n + 1):
            x  = x0 + i * step
            y += random.uniform(-s(3), s(3))
            y  = y_nom + max(-amp * 2, min(amp * 2, y - y_nom))
            sp = amp * 2.2 if random.random() < 0.07 else 0
            pts.append((x, y + random.uniform(-amp, amp) + random.choice([-1, 1]) * sp))
        return pts

    top_edge = torn_pts(PL, PR, PT, amp=s(24))
    bot_edge = torn_pts(PL, PR, PB, amp=s(24))

    paper_poly = (top_edge +
                  [(PR, PB + s(30)), (PR, PB - s(30))] +
                  list(reversed(bot_edge)) +
                  [(PL, PT + s(30)), (PL, PT - s(30))])

    # ── Canvas ───────────────────────────────────────────────────────────────
    img  = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Shadow
    sh = [(x + s(9), y + s(9)) for x, y in paper_poly]
    draw.polygon(sh, fill=(0, 0, 0, 120))

    # Paper body
    draw.polygon(paper_poly, fill=PAPER)

    # Horizontal rule lines
    for y in range(PT + s(55), PB - s(30), s(32)):
        draw.line([(PL + s(15), y), (PR - s(15), y)], fill=PAPER_MID, width=1)

    # Red margin line
    MX = PL + s(88)
    draw.line([(MX, PT + s(40)), (MX, PB - s(40))], fill=RULE_RED, width=s(2))

    # Torn-edge fibers
    def draw_fibers(edge_pts, direction):
        for i in range(0, len(edge_pts) - 1, 2):
            x, ey = edge_pts[i]
            length = random.randint(s(4), s(18))
            draw.line([(x, ey),
                       (x + random.uniform(-s(3), s(3)), ey + direction * length)],
                      fill=TORN_FIBER, width=1)

    draw_fibers(top_edge, -1)
    draw_fibers(bot_edge,  1)

    # Torn-edge highlight strip
    for pts, dy in [(top_edge, +s(6)), (bot_edge, -s(6))]:
        shifted = [(x, y + dy) for x, y in pts]
        for i in range(len(shifted) - 1):
            draw.line([shifted[i], shifted[i + 1]], fill=PAPER_DARK, width=s(3))

    # ── Layout helpers ───────────────────────────────────────────────────────
    CX = (PL + PR) // 2
    TX = MX + s(28)
    RX = PR - s(30)

    def centre_text(text, font, colour, y_pos):
        bb = draw.textbbox((0, 0), text, font=font)
        draw.text((CX - (bb[2] - bb[0]) // 2, y_pos), text, font=font, fill=colour)
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
        y_pos += (bb[3] - bb[1]) + s(12)
        draw.line([(TX, y_pos), (TX + (bb[2] - bb[0]) + s(10), y_pos)],
                  fill=HEADER_INK, width=s(2))
        return y_pos + s(12)

    def key_val(key, val, y_pos, gap=s(30)):
        draw.text((TX + s(16), y_pos), key, font=F_BODY_B, fill=INK)
        kbb = draw.textbbox((0, 0), key, font=F_BODY_B)
        draw.text((TX + s(16) + (kbb[2] - kbb[0]) + gap, y_pos), val, font=F_BODY, fill=INK)
        return y_pos + (kbb[3] - kbb[1]) + s(14)

    layout = dict(
        draw=draw, CX=CX, TX=TX, RX=RX, PL=PL, PR=PR, PT=PT, PB=PB,
        centre_text=centre_text, left_text=left_text, divider=divider,
        section_header=section_header, key_val=key_val,
    )

    # ── Call the page-specific content ───────────────────────────────────────
    content_fn(layout, PT + s(52))

    img.save(out_path)
    print(f"Saved -> {out_path}  ({W}x{H} px)")


# ─────────────────────────────────────────────────────────────────────────────
#  Page 1 content:  Title  /  How to Play  /  Scoring  /  Hazards
# ─────────────────────────────────────────────────────────────────────────────
def page1_content(L, y):
    draw          = L["draw"]
    CX            = L["CX"]
    centre_text   = L["centre_text"]
    left_text     = L["left_text"]
    divider       = L["divider"]
    section_header = L["section_header"]
    key_val       = L["key_val"]

    # Decorative title lines
    for i, thick in enumerate([1, 2]):
        lx = CX - s(280) + i * s(5)
        rx = CX + s(280) - i * s(5)
        draw.line([(lx, y + i * s(5)), (rx, y + i * s(5))], fill=RULE_BLUE, width=thick)
    y += s(16)

    y = centre_text("NO MAN'S ROAD",   F_TITLE_B,  TITLE_INK,  y)
    y = centre_text("Player's Manual", F_SUBTITLE, HEADER_INK, y)

    for i, thick in enumerate([2, 1]):
        lx = CX - s(280) + i * s(5)
        rx = CX + s(280) - i * s(5)
        draw.line([(lx, y + i * s(5)), (rx, y + i * s(5))], fill=RULE_BLUE, width=thick)
    y += s(26)

    # How to Play
    y = section_header("HOW TO PLAY", y)
    y = key_val("Left Arrow",  "Switch Lane Left",  y)
    y = key_val("Right Arrow", "Switch Lane Right", y)
    y = key_val("Space",       "Jump",              y)
    y = left_text("Run through 3 lanes, dodge obstacles, collect", F_BODY, INK, y, indent=s(16))
    y = left_text("items, and survive as long as possible!",       F_BODY, INK, y, indent=s(16))
    y = divider(y)

    # Scoring
    y = section_header("SCORING", y)
    y = key_val("Egg  (common)", "1 pt  x  Multiplier",  y)
    y = key_val("Gem  (rare)",   "10 pts  x  Multiplier", y)
    y = left_text("Your score is shown live at the top of the screen.", F_BODY, INK, y, indent=s(16))
    y = divider(y)

    # Hazards
    y = section_header("HAZARDS", y)
    y = key_val("Car  -  front hit", "-50  health", y)
    y = key_val("Car  -  side hit",  "-15  health", y)
    y = key_val("Dog",               "-20  health", y)
    y = key_val("Tank Bomb",         "-30  health", y)
    y = left_text("Jump over cars or switch lanes to avoid damage.", F_BODY, INK, y, indent=s(16))


# ─────────────────────────────────────────────────────────────────────────────
#  Page 2 content:  Power-Ups  /  General Info  /  Footer
# ─────────────────────────────────────────────────────────────────────────────
def page2_content(L, y):
    draw           = L["draw"]
    PR             = L["PR"]
    PB             = L["PB"]
    centre_text    = L["centre_text"]
    left_text      = L["left_text"]
    divider        = L["divider"]
    section_header = L["section_header"]
    key_val        = L["key_val"]

    y += s(10)   # slight top padding on page 2

    # Power-Ups
    y = section_header("POWER-UPS", y)
    y = key_val("[*]  Score Multiplier", "Lasts 20 seconds", y)
    y = left_text("Pick up the multiplier icon to stack a +1 bonus",     F_BODY, INK, y, indent=s(32))
    y = left_text("onto every future score. Stacks with more pickups!",  F_BODY, INK, y, indent=s(32))
    y = divider(y)

    # General Info
    y = section_header("GENERAL INFO", y)
    y = left_text("- You start with 100 health points.",            F_BODY, INK, y, indent=s(16))
    y = left_text("- Game ends when health reaches zero.",           F_BODY, INK, y, indent=s(16))
    y = left_text("- Health is shown as a slider on-screen.",       F_BODY, INK, y, indent=s(16))
    y = left_text("- Use Restart or Quit on the Game Over screen.", F_BODY, INK, y, indent=s(16))
    y = left_text("- The road speeds up as obstacles increase.",    F_BODY, INK, y, indent=s(16))

    y = divider(y, gap=s(24))
    y = centre_text("Version 1.0  -  No Man's Road  -  FinalGameC",
                    F_SMALL, (140, 110, 70), y + s(10))

    # Coffee-stain ring
    for r in [s(55), s(52), s(49)]:
        cx2, cy2 = PR - s(110), PB - s(110)
        draw.ellipse([(cx2 - r, cy2 - r), (cx2 + r, cy2 + r)],
                     outline=(160, 120, 70), width=s(2))


# ─────────────────────────────────────────────────────────────────────────────
#  Generate both pages
# ─────────────────────────────────────────────────────────────────────────────
W = s(1200)

build_page(seed=7,  W=W, H=s(1200), content_fn=page1_content,
           out_path=os.path.join(DIR, "HelpPage_1.png"))

build_page(seed=13, W=W, H=s(800),  content_fn=page2_content,
           out_path=os.path.join(DIR, "HelpPage_2.png"))

print("Done.")
