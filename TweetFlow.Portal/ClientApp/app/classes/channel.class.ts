import { Tweet } from "./classes";

export class Channel {
    name: string;
    endpoint: string;
    closed: boolean;
    column: number;
    tweets: Tweet[];

    constructor() {
        this.tweets = new Array<Tweet>();
    }
}