process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = "0";
export default {
  api: {
    input: "https://localhost:5001/openapi.json",
    output: {
      target: "./src/api/generated.ts",
      client: "axios",
      override: {
        mutator: {
            path: "./src/client/axiosInstance.ts",
            name: "axiosInstance",
            isRaw: true,
        },
      },
      clean: true,
    },
  },
};