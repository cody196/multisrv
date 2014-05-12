﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources
{
    public class ResourceManager
    {
        private Dictionary<string, Resource> m_resources;

        public ResourceManager()
        {
            m_resources = new Dictionary<string, Resource>();
        }

        public Resource GetResource(string name)
        {
            Resource res;

            if (m_resources.TryGetValue(name, out res))
            {
                return res;
            }

            return null;
        }

        public Resource AddResource(string name, string path)
        {
            var res = new Resource(name, path);

            if (res.Parse())
            {
                AddResource(res);

                return res;
            }

            return null;
        }

        public IEnumerable<Resource> GetRunningResources()
        {
            return from r in m_resources
                   where r.Value.State == ResourceState.Running
                   select r.Value;
        }

        public void AddResource(Resource res)
        {
            res.Manager = this;

            m_resources[res.Name] = res;
        }

        public void ScanResources(string path)
        {
            var subdirs = Directory.GetDirectories(path);

            foreach (var dir in subdirs)
            {
                var basename = Path.GetFileName(dir);

                if (basename[0] == '[')
                {
                    if (!basename.Contains(']'))
                    {
                        this.Log().Info("Ignored {0} - no end bracket", basename);
                        continue;
                    }

                    ScanResources(dir);
                }
                else
                {
                    AddResource(basename, dir);
                }
            }
        }

        public void ScanResources(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                ScanResources(path);
            }
        }
    }
}
