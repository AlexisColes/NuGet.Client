// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Xunit;

namespace NuGet.Protocol.Tests
{
    public class DownloadResourceResultTests
    {
        [Fact]
        public void Constructor_Status_ThrowsIfStatusIsAvailable()
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new DownloadResourceResult(DownloadResourceResultStatus.Available));

            Assert.Equal("status", exception.ParamName);
        }

        [Theory]
        [InlineData(DownloadResourceResultStatus.Cancelled)]
        [InlineData(DownloadResourceResultStatus.NotFound)]
        public void Constructor_Status_InitializesProperties(DownloadResourceResultStatus status)
        {
            using (var result = new DownloadResourceResult(status))
            {
                Assert.Null(result.PackageReader);
                Assert.Null(result.PackageSource);
                Assert.Null(result.PackageStream);
                Assert.Equal(status, result.Status);
            }
        }

        [Fact]
        public void Constructor_Stream_ThrowsForNullStream()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new DownloadResourceResult(stream: null));

            Assert.Equal("stream", exception.ParamName);
        }

        [Fact]
        public void Constructor_Stream_InitializesProperties()
        {
            using (var result = new DownloadResourceResult(Stream.Null))
            {
                Assert.Null(result.PackageReader);
                Assert.Null(result.PackageSource);
                Assert.Same(Stream.Null, result.PackageStream);
                Assert.Equal(DownloadResourceResultStatus.Available, result.Status);
            }
        }

        [Fact]
        public void Constructor_StreamSource_ThrowsForNullStream()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new DownloadResourceResult(stream: null, source: "a"));

            Assert.Equal("stream", exception.ParamName);
        }

        [Fact]
        public void Constructor_StreamSource_AllowsNullSource()
        {
            using (var result = new DownloadResourceResult(Stream.Null, source: null))
            {
                Assert.Null(result.PackageSource);
            }
        }

        [Fact]
        public void Constructor_StreamSource_InitializesProperties()
        {
            using (var result = new DownloadResourceResult(Stream.Null, source: "a"))
            {
                Assert.Null(result.PackageReader);
                Assert.Equal("a", result.PackageSource);
                Assert.Same(Stream.Null, result.PackageStream);
                Assert.Equal(DownloadResourceResultStatus.Available, result.Status);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBase_ThrowsForNullStream()
        {
            using (var packageReader = new TestPackageReader())
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => new DownloadResourceResult(stream: null, packageReader: packageReader));

                Assert.Equal("stream", exception.ParamName);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBase_AllowsNullPackageReader()
        {
            using (var result = new DownloadResourceResult(Stream.Null, packageReader: null))
            {
                Assert.Null(result.PackageReader);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBase_InitializesProperties()
        {
            using (var packageReader = new TestPackageReader())
            using (var result = new DownloadResourceResult(Stream.Null, packageReader))
            {
                Assert.Same(packageReader, result.PackageReader);
                Assert.Null(result.PackageSource);
                Assert.Same(Stream.Null, result.PackageStream);
                Assert.Equal(DownloadResourceResultStatus.Available, result.Status);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBaseSource_ThrowsForNullStream()
        {
            using (var packageReader = new TestPackageReader())
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => new DownloadResourceResult(stream: null, packageReader: packageReader, source: "a"));

                Assert.Equal("stream", exception.ParamName);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBaseSource_AllowsNullPackageReaderAndSource()
        {
            using (var result = new DownloadResourceResult(Stream.Null, packageReader: null, source: null))
            {
                Assert.Null(result.PackageReader);
                Assert.Null(result.PackageSource);
            }
        }

        [Fact]
        public void Constructor_StreamPackageReaderBaseSource_InitializesProperties()
        {
            using (var packageReader = new TestPackageReader())
            using (var result = new DownloadResourceResult(Stream.Null, packageReader, source: "a"))
            {
                Assert.Same(packageReader, result.PackageReader);
                Assert.Equal("a", result.PackageSource);
                Assert.Same(Stream.Null, result.PackageStream);
                Assert.Equal(DownloadResourceResultStatus.Available, result.Status);
            }
        }

        private sealed class TestPackageReader : PackageReaderBase
        {
            public TestPackageReader()
                : base(DefaultFrameworkNameProvider.Instance, DefaultCompatibilityProvider.Instance)
            {
            }

            public override IEnumerable<string> CopyFiles(
                string destination,
                IEnumerable<string> packageFiles,
                ExtractPackageFileDelegate extractFile,
                ILogger logger,
                CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetFiles()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetFiles(string folder)
            {
                throw new NotImplementedException();
            }

            public override Stream GetStream(string path)
            {
                throw new NotImplementedException();
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}