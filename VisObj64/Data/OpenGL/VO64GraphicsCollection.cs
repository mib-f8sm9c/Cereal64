﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public class VO64GraphicsCollection
    {
        public ReadOnlyCollection<VO64GraphicsElement> Elements
        {
            get { return _elements.AsReadOnly(); }
        }
        private List<VO64GraphicsElement> _elements;

        public ReadOnlyCollection<VO64GraphicsCollection> Collections
        {
            get { return _collections.AsReadOnly(); }
        }
        private List<VO64GraphicsCollection> _collections;

        public VO64GraphicsCollection()
        {
            _elements = new List<VO64GraphicsElement>();
            _collections = new List<VO64GraphicsCollection>();
        }

        public void Add(VO64GraphicsElement element)
        {
            _elements.Add(element);
        }

        public void Add(VO64GraphicsCollection collection)
        {
            _collections.Add(collection);
        }

        public void Remove(VO64GraphicsElement element)
        {
            if (_elements.Contains(element))
                _elements.Remove(element);
            element.Dispose();
        }

        public void Remove(VO64GraphicsCollection collection)
        {
            if (_collections.Contains(collection))
                _collections.Remove(collection);
            collection.Dispose();
        }

        public void Draw()
        {
            //Draw all elements
            foreach (VO64GraphicsElement element in _elements)
                element.Draw();

            foreach (VO64GraphicsCollection collection in _collections)
                collection.Draw();
        }

        public void Dispose()
        {
            //Remove all elements
            foreach (VO64GraphicsElement element in _elements)
                element.Dispose();

            _elements.Clear();


            foreach (VO64GraphicsCollection collection in _collections)
                collection.Dispose();

            _collections.Clear();
        }
    }
}
