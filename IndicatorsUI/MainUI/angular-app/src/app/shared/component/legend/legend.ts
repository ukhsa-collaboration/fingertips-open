import { PageType, ComparatorMethodIds, PolarityIds } from "../../constants";
import { FTHelperService } from "../../service/helper/ftHelper.service";
import { GroupRoot } from "../../../typings/FT";

export class LegendConfig {
    public showRAG3 = false;
    public showRAG5 = false;
    public showBOB3 = false;
    public showBOB5 = false;
    public showQuintilesRAG = false;
    public showQuintilesBOB = false;
    public showQuartiles = false;
    public showContinuous = false;
    public showBenchmarkAgainstGoal = false;
    public targetLegendForBenchmark = '';

    constructor(public pageType: PageType, public ftHelper: FTHelperService) { }

    public isBenchmarkAgainstGoalSelected(): boolean {
        return this.showBenchmarkAgainstGoal
            && this.targetLegendForBenchmark.length > 0;
    }

    public isAnySignificanceColouring(): boolean {
        return !this.isBenchmarkAgainstGoalSelected()
            && (this.showComparedToBenchmarkLegend() || this.showQuintiles());
    }

    public showComparedToBenchmarkLegend(): boolean {
        return !this.isBenchmarkAgainstGoalSelected()
            && (this.showRAG3 || this.showRAG5 || this.showBOB3 || this.showBOB5);
    }

    public showQuintiles(): boolean {
        return !this.isBenchmarkAgainstGoalSelected()
            && (this.showQuintilesBOB || this.showQuintilesRAG);
    }

    public showNotComparedOnly(): boolean {
        return !this.isBenchmarkAgainstGoalSelected()
            && !this.isAnySignificanceColouring()
            && !this.showQuartiles
            && !this.showContinuous;
    }

    public configureBenchmarkAgainstGoal(showBenchmarkAgainstGoal: boolean, targetLegendForBenchmark: string) {
        this.showBenchmarkAgainstGoal = showBenchmarkAgainstGoal;
        this.targetLegendForBenchmark = targetLegendForBenchmark;
    }

    // compare area, map, inequalities
    public configureForOneIndicator(groupRoot: GroupRoot) {
        const comparatorMethodId = groupRoot.ComparatorMethodId;
        const polarityId = groupRoot.PolarityId;

        if (comparatorMethodId === ComparatorMethodIds.Quintiles) {
            // Show Quintile legend
            if (polarityId === PolarityIds.NotApplicable) {
                this.showQuintilesBOB = true;
            } else {
                this.showQuintilesRAG = true;
            }
        } else {
            if (comparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels) {
                switch (polarityId) {
                    case PolarityIds.RAGLowIsGood:
                        // Show RAG5 legend
                        this.showRAG5 = true;
                        break;
                    case PolarityIds.RAGHighIsGood:
                        // Show RAG5 legend
                        this.showRAG5 = true;
                        break;
                    case PolarityIds.BlueOrangeBlue:
                        // Show BOB legend
                        this.showBOB5 = true;
                        break;
                }
            } else {
                switch (polarityId) {
                    case PolarityIds.RAGLowIsGood:
                        // Show RAG3 legend
                        this.showRAG3 = true;
                        break;
                    case PolarityIds.RAGHighIsGood:
                        // Show RAG3 legend
                        this.showRAG3 = true;
                        break;
                    case PolarityIds.BlueOrangeBlue:
                        // Show BOB legend
                        this.showBOB3 = true;
                        break;
                }
            }
        }
    }

    // trends, area profile
    public configureForMultipleIndicators(groupRoots: GroupRoot[]) {
        // Show Quintile BOB legend
        if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.Quintiles
            && (x.PolarityId === PolarityIds.NotApplicable)) > -1) {

            this.showQuintilesBOB = true;
        }

        // Show Quintile RAG legend
        if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.Quintiles
            && (x.PolarityId === PolarityIds.RAGLowIsGood ||
                x.PolarityId === PolarityIds.RAGHighIsGood ||
                x.PolarityId === PolarityIds.BlueOrangeBlue)) > -1) {

            this.showQuintilesRAG = true;
        }

        if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels
            && (x.PolarityId === PolarityIds.RAGLowIsGood || x.PolarityId === PolarityIds.RAGHighIsGood)) > -1) {
            // Show RAG5 legend
            this.showRAG5 = true;
        } else if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels
            && (x.PolarityId === PolarityIds.BlueOrangeBlue)) > -1) {
            // Show BOB5 legend
            this.showBOB5 = true;

        } else {
            // Show RAG3 legend
            if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel) > -1) {
                this.showRAG3 = true;
            }

            if (this.showRAG3 === false &&
                (groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGHighIsGood) > -1 ||
                    groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGLowIsGood) > -1)) {

                this.showRAG3 = true;
            }
        }

        // Show BOB legend
        if (this.showBOB5 === false &&
            groupRoots.findIndex(x => x.PolarityId === PolarityIds.BlueOrangeBlue &&
                x.ComparatorMethodId !== ComparatorMethodIds.Quintiles) > -1) {

            this.showBOB3 = true;
        }
    }
}
