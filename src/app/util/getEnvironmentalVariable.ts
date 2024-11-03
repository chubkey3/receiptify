export function getEnvVariable(key: string) {
    const value = process.env[key];

    if (value) {
        return value;
    } else {
        throw new Error(`Environmental variable ${key} is undefined`);
    }
}