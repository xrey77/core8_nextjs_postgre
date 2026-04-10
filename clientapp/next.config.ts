import type { NextConfig } from "next";
import path from 'path';

const nextConfig: NextConfig = {
  outputFileTracingRoot: path.join(__dirname, '../../'),
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'localhost', 
        port: '7292',
        pathname: '/**'
      },
      {
        protocol: 'https',
        hostname: '127.0.0.1', 
        port: '7292',
        pathname: '/**'
      },
    ],
  },
  // Optional: Next 16 specific features
  // cacheComponents: true, 
};

export default nextConfig;
