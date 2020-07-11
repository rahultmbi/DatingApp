import { Photo } from './photo';

export interface User {
    id: number;
    usename: string;
    age: number;
    knownAs: string;
    gender: string;
    createdOn: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
