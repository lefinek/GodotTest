using Godot;

public partial class ProductDetails : Node
{
    // WARNING: Storing secrets in source code is insecure. Consider loading from encrypted config or environment.
    [Export] public string product_id { get; set; } = "e0fad88fbfc147ddabce0900095c4f7b";
    [Export] public string sandbox_id { get; set; } = "ce451c8e18ef4cb3bc7c5cdc11a9aaae";
    [Export] public string deployment_id { get; set; } = "0e28b5f3257a4dbca04ea0ca1c30f265"; // trimmed trailing space
    [Export] public string client_id { get; set; } = "xyza7891eEYHFtDWNZaFlmauAplnUo5H";
    [Export] public string client_secret { get; set; } = "xD8rxykYUyqoaGoYZ5zhK+FD6Kg8+LvkATNkDb/7DPo";
    [Export] public string encryption_key { get; set; } = ""; // 64 char hex recommended.
}
