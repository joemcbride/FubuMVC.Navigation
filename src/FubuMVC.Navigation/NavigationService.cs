using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;

namespace FubuMVC.Navigation
{
    public class NavigationService : INavigationService
    {
	    private readonly IChainUrlResolver _urlResolver;
        private readonly IMenuStateService _stateService;
	    private readonly IAssetUrls _urls;

        private readonly NavigationGraph _navigation;

		public NavigationService(BehaviorGraph graph, IChainUrlResolver urlResolver, IMenuStateService stateService, IAssetUrls urls)
		{
			_urlResolver = urlResolver;
            _stateService = stateService;
            _urls = urls;
            _navigation = graph.Settings.Get<NavigationGraph>();
        }

        public IEnumerable<MenuItemToken> MenuFor(StringToken key)
        {
            var chain = _navigation.MenuFor(key);
            return chain.Select(BuildToken);
        }


        // TODO -- this could really use some more end to end testing
        public MenuItemToken BuildToken(MenuNode node)
        {
            var token = new MenuItemToken {
                Children = node.Children.Select(BuildToken).ToArray(),
                Key = node.Key.Key,
                Text = node.Key.ToString(),
                Category = node.Category,
                MenuItemState = _stateService.DetermineStateFor(node)
            };

            if (node.Icon().IsNotEmpty())
            {
                token.IconUrl = _urls.UrlForAsset(AssetFolder.images, node.Icon());
            }

            if (node.Type == MenuNodeType.Leaf)
            {
	            token.Url = _urlResolver.UrlFor(node.UrlInput, node.BehaviorChain);
            }

			node.ForData(token.Set);

            return token;
        }
    }
}