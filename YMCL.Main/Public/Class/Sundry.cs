﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Main.Public.Class
{
    public class Hitokoto
    {
        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? uuid { get; set; }
            /// <summary>
            /// 就很犯困很困很困~很忙还是很困！
            /// </summary>
            public string? hitokoto { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? type { get; set; }
            /// <summary>
            /// 上班
            /// </summary>
            public string? from { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? from_who { get; set; }
            /// <summary>
            /// 美美的鱼香肉丝
            /// </summary>
            public string? creator { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int creator_uid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int reviewer { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? commit_from { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? created_at { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int length { get; set; }
        }
    }
    public class DownloadFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string MD5 { get; set; }
    }
    public class AccountInfo
    {
        public SettingItem.AccountType AccountType { get; set; }

        public string Name { get; set; } = "Unnamed";

        public string AddTime { get; set; } = "1970-01-01T00:00:00+08:00";

        public string? Data { get; set; }

        public string Skin { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAFDUlEQVR42u2a20sUURzH97G0LKMotPuWbVpslj1olJXdjCgyisowsSjzgrB0gSKyC5UF1ZNQWEEQSBQ9dHsIe+zJ/+nXfM/sb/rN4ZwZ96LOrnPgyxzP/M7Z+X7OZc96JpEISfWrFhK0YcU8knlozeJKunE4HahEqSc2nF6zSEkCgGCyb+82enyqybtCZQWAzdfVVFgBJJNJn1BWFgC49/VpwGVlD0CaxQiA5HSYEwBM5sMAdKTqygcAG9+8coHKY/XXAZhUNgDYuBSPjJL/GkzVVhAEU5tqK5XZ7cnFtHWtq/TahdSw2l0HUisr1UKIWJQBAMehDuqiDdzndsP2EZECAG1ZXaWMwOCODdXqysLf++uXUGv9MhUHIByDOijjdiSAoH3ErANQD73C7TXXuGOsFj1d4YH4OTJAEy8y9Hd0mCaeZ5z8dfp88zw1bVyiYhCLOg1ZeAqC0ybaDttHRGME1DhDeVWV26u17lRAPr2+mj7dvULfHw2q65fhQRrLXKDfIxkau3ZMCTGIRR3URR5toU38HbaPiMwUcKfBAkoun09PzrbQ2KWD1JJaqswjdeweoR93rirzyCMBCmIQizqoizZkm2H7iOgAcHrMHbbV9KijkUYv7qOn55sdc4fo250e+vUg4329/Xk6QB/6DtOws+dHDGJRB3XRBve+XARt+4hIrAF4UAzbnrY0ve07QW8uHfB+0LzqanMM7qVb+3f69LJrD90/1axiEIs6qIs21BTIToewfcSsA+Bfb2x67OoR1aPPzu2i60fSNHRwCw221Suz0O3jO+jh6V1KyCMGse9721XdN5ePutdsewxS30cwuMjtC860T5JUKpXyKbSByUn7psi5l+juDlZYGh9324GcPKbkycaN3jUSAGxb46IAYPNZzW0AzgiQ5tVnzLUpUDCAbakMQXXrOtX1UMtHn+Q9/X5L4wgl7t37r85OSrx+TYl379SCia9KXjxRpiTjIZTBFOvrV1f8ty2eY/T7XJ81FQAwmA8ASH1ob68r5PnBsxA88/xAMh6SpqW4HRnLBrkOA9Xv5wPAZjAUgOkB+SHxgBgR0qSMh0zmZRsmwDJm1gFg2PMDIC8/nAHIMls8x8GgzOsG5WiaqREgYzDvpTwjLDy8NM15LpexDEA3LepjU8Z64my+8PtDCmUyRr+fFwA2J0eAFYA0AxgSgMmYBMZTwFQnO9RNAEaHOj2DXF5UADmvAToA2ftyxZYA5BqgmZZApDkdAK4mAKo8GzPlr8G8AehzMAyA/i1girUA0HtYB2CaIkUBEHQ/cBHSvwF0AKZFS5M0ZwMQtEaEAmhtbSUoDADH9ff3++QZ4o0I957e+zYAMt6wHkhzpjkuAcgpwNcpA7AZDLsvpwiuOkBvxygA6Bsvb0HlaeKIF2EbADZpGiGzBsA0gnwQHGOhW2snRpbpPexbAB2Z1oicAMQpTnGKU5ziFKc4xSlOcYpTnOIUpzgVmgo+XC324WfJAdDO/+ceADkCpuMFiFKbApEHkOv7BfzfXt+5gpT8V7rpfYJcDz+jAsB233r6yyBsJ0mlBCDofuBJkel4vOwBFPv8fyYAFPJ+wbSf/88UANNRVy4Awo6+Ig2gkCmgA5DHWjoA+X7AlM//owLANkX0w0359od++pvX8fdMAcj3/QJ9iJsAFPQCxHSnQt8vMJ3v2wCYpkhkAOR7vG7q4aCXoMoSgG8hFAuc/grMdAD4B/kHl9da7Ne9AAAAAElFTkSuQmCC";
    }
    public class _2018k
    {
        public bool EnabledGithubApi {  get; set; }
        public string GithubApi {  get; set; }
        public string? X64 { get; set; }
        public string? X86 { get; set; }
        public InfoBar Notice { get; set; }
        public class InfoBar
        {
            public InfoType Type { get; set; }
            public string? Msg { get; set; }
            public enum InfoType
            {
                Informational,
                Success,
                Warning,
                Error
            }
        }
    }
    public class ModListViewEntry()
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string DownloadCount { get; set; }
        public string DateModified { get; set; }
        public string Source { get; set; }
        public string IconUrl { get; set; }
    }
}
