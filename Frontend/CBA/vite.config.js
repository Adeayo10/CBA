/* eslint-disable no-unused-vars */
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

/*https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
})
 eslint-disable no-unused-vars 
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'*/
// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  optimizeDeps: {
    include: ["@emotion/styled", "react-router-dom"],
  },
  server: {
    //port: 7004,  // Client port
    proxy: {
      "/api": {
        target: "http://localhost:5256", // Mock server port
        changeOrigin: true,
        secure: false,
        ws: true,
        configure: (proxy, _options) => {
          proxy.on("error", (err, _req, _res) => {
            console.log("proxy error", err);
          });
          proxy.on("proxyReq", (proxyReq, req, _res) => {
            console.log("Sending Request to the Target:", req.method, req.url);
          });
          proxy.on("proxyRes", (proxyRes, req, _res) => {
            console.log(
              "Received Response from the Target:",
              proxyRes.statusCode,
              req.url
            );
          });
        },
      },
    },
  },
});
