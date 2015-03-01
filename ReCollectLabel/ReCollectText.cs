﻿using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using CoreGraphics;

namespace ReCollect
{
	public partial class ReColor {
		public ReColor (UIColor color) {
			nfloat red, green, blue, alpha;
			color.GetRGBA (out red, out green, out blue, out alpha);
			R = (int)(255 * red);
			G = (int)(255 * green);
			B = (int)(255 * blue);
			A = (int)(255 * alpha);
		}

		public UIColor UIColor {
			get {
				return new UIColor (R / 255.0f, G / 255.0f, B / 255.0f, 1.0f);
			}
		}
	}

	public partial class StyleWithRange {
		public UIStringAttributes Attributes;
	}

	public partial class ReText
	{
		public CGSize GetBoundedSize (CGSize max) {
			return AttributedText.GetBoundingRect (
				max,
				NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading,
				null
			).Size;
		}

		NSMutableAttributedString _AttributedText = null;
		public NSMutableAttributedString AttributedText {
			get {
				if (_AttributedText != null)
					return _AttributedText;

				var parsed = ParseHtml ();

				// Apply attributes
				_AttributedText = new NSMutableAttributedString (parsed.Text);
				_AttributedText.BeginEditing ();
				while (parsed.Styles.Count > 0) {
					var ranged_attrs = parsed.Styles.Pop ();
					var attributes = (NSMutableDictionary) ranged_attrs.Style.Attributes.Dictionary;

					if (attributes ["NSLink"] != null) {
						attributes ["Link"] = attributes ["NSLink"];
						attributes.Remove (new NSString ("NSLink"));
					}

					_AttributedText.AddAttributes (
						attributes,
						new NSRange (ranged_attrs.Offset, ranged_attrs.Length)
					);
				}
				_AttributedText.EndEditing ();

				return _AttributedText;
			}
		}

		partial class TextStyle {
			public virtual UIStringAttributes Attributes { get; set; }
		}

		partial class HeaderStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						Font = UIFont.FromName (Text.FontName, Text.FontSize * Factor),
						ForegroundColor = Text.TextColor.UIColor
					};
				}
			}
		}

		partial class LinkStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						ForegroundColor = Text.LinkColor.UIColor,
						UnderlineColor = Text.LinkColor.UIColor,
						UnderlineStyle = NSUnderlineStyle.Single,
						Link = NSUrl.FromString (Href)
					};
				}
			}
		}

		partial class ItalicStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						Font = UIFont.FromName (Text.ItalicsFontName, Text.FontSize)
					};
				}
			}
		}

		partial class BoldStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						Font = UIFont.FromName (Text.BoldFontName, Text.FontSize)
					};
				}
			}
		}

		partial class ColorStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						ForegroundColor = Color.UIColor
					};
				}
			}
		}

		partial class FontStyle : TextStyle {
			override public UIStringAttributes Attributes {
				get {
					return new UIStringAttributes () {
						Font = UIFont.FromName (FontName, Text.FontSize),
						ForegroundColor = Text.TextColor.UIColor
					};
				}
			}
		}
	}
}

