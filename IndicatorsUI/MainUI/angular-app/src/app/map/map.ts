declare namespace geoBoundry{

    export interface Properties {
        AreaCode: string;
    }

    export interface Geometry {
        type: string;
        coordinates: number[][][][];
    }

    export interface Feature {
        type: string;
        properties: Properties;
        geometry: Geometry;
    }

    export interface Boundry {
        type: string;
        features: Feature[];
    }
}
