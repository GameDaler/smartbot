using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Smart_Bot
{
	public class UrlButton
	{
		public string Text { get; set; }
		public string CallbackData { get; set; }
		public UrlButton(string text, string callbackData)
		{
			Text = text;
			CallbackData = callbackData;
		}
		public UrlButton() { }
        public InlineKeyboardButton ToUrlButton()
		{
			return InlineKeyboardButton.WithUrl(Text, CallbackData);
		}
	}
	public class UrlButtonKeyboard
	{
		public List<UrlButton> urlButtons { get; set; } = new List<UrlButton>();

		public UrlButtonKeyboard(params UrlButton[] buttons)
		{
			urlButtons.AddRange(buttons);
		}
		public UrlButtonKeyboard() { }

        public void Add(params UrlButton[] buttons)
		{
			urlButtons.AddRange(buttons);
		}

		public InlineKeyboardMarkup ToInlineKeyboard()
		{
			var inlineKeyboard = new List<List<InlineKeyboardButton>>();
			foreach (var button in urlButtons)
			{
				inlineKeyboard.Add(new List<InlineKeyboardButton> { button.ToUrlButton() });
			}
			return new InlineKeyboardMarkup(inlineKeyboard);
		}
	}
}
