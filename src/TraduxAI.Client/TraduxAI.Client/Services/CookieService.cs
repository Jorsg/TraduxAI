﻿using Microsoft.JSInterop;

namespace TraduxAI.Client.Services
{
    public class CookieService
    {
        private readonly IJSRuntime _jsRuntime;
        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task SetCookie(string key, string value, int days)
        {

            await _jsRuntime.InvokeVoidAsync("setCookie", key, value, days);
        }
        public async Task<string> GetCookie(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("getCookie", key);

        }
        public async Task DeleteCookie(string key)
        {
            await _jsRuntime.InvokeVoidAsync("deleteCookie", key);
        }
    }
}
