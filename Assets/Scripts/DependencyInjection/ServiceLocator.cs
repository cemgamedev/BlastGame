using System;
using StickBlast.Core.Interfaces;
using StickBlast.Implementation;

namespace StickBlast.Core.DependencyInjection
{
    public class ServiceLocator
    {
        private static ServiceLocator instance;
        public static ServiceLocator Instance => instance ??= new ServiceLocator();

        private readonly IItemAnimationController itemAnimationController;
        private readonly IItemSpawner itemSpawner;

        private ServiceLocator()
        {
            itemAnimationController = new ItemAnimationController();
            itemSpawner = new ItemSpawner();
        }

        public IItemAnimationController GetItemAnimationController() => itemAnimationController;
        public IItemSpawner GetItemSpawner() => itemSpawner;
    }
} 