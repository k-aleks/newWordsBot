using System;
using FluentAssertions;
using NewWordsBot;
using NSubstitute;
using Xunit;

namespace NewWordsBotTests
{
    public class LearningMethodologyTests
    {
        private readonly DateTime timeProviderInOneMinuteResult = DateTime.Today.AddMinutes(1);
        private readonly DateTime timeProviderInThirtyMinutesResult = DateTime.Today.AddMinutes(30);
        private readonly DateTime timeProviderInOneDayResult = DateTime.Today.AddDays(1);
        private readonly DateTime timeProviderInForteenDaysResult = DateTime.Today.AddDays(14);
        private readonly DateTime timeProviderInSixtyDaysResult = DateTime.Today.AddDays(60);

        private readonly DateTime[] timeProviderResults;

        public LearningMethodologyTests()
        {
            timeProviderResults = new[]
            {
                timeProviderInOneMinuteResult,
                timeProviderInThirtyMinutesResult,
                timeProviderInOneDayResult,
                timeProviderInForteenDaysResult,
                timeProviderInSixtyDaysResult,
                DateTime.MaxValue,
            };
        }

        [Theory]
        [InlineData(LearningStage.First_1m, LearningStage.Second_30m, 1)]
        [InlineData(LearningStage.Second_30m, LearningStage.Third_24h, 2)]
        [InlineData(LearningStage.Third_24h, LearningStage.Forth_14d, 3)]
        [InlineData(LearningStage.Forth_14d, LearningStage.Fifth_60d, 4)]
        [InlineData(LearningStage.Fifth_60d, LearningStage.Sixth_Completed, 5)]
        public void OnRightResponse_should_promote_learning_progress(LearningStage currentStage, LearningStage resultStage, int expectedNextRepetitionIndex)
        {
            var timeProvider = CreateTimeProviderMock();
            var lm = new LearningMethodology(timeProvider);
            
            var resultWord = lm.OnRightResponse(CreateWord(currentStage));

            resultWord.Stage.Should().Be(resultStage);
            resultWord.NextRepetition.Should().Be(timeProviderResults[expectedNextRepetitionIndex]);
        }

        [Fact]
        public void OnWrongResponse_should_reset_learning_progress()
        {
            var timeProvider = CreateTimeProviderMock();
            var lm = new LearningMethodology(timeProvider);
            
            var resultWord = lm.OnWrongResponse(CreateWord(LearningStage.Second_30m));

            resultWord.Stage.Should().Be(LearningStage.First_1m);
            resultWord.NextRepetition.Should().Be(timeProviderInOneMinuteResult);
            timeProvider.Received().InOneMinute();
        }

        private ITimeProvider CreateTimeProviderMock()
        {
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.InOneMinute().Returns(timeProviderInOneMinuteResult);
            timeProvider.InThirtyMinutes().Returns(timeProviderInThirtyMinutesResult);
            timeProvider.InOneDay().Returns(timeProviderInOneDayResult);
            timeProvider.InForteenDays().Returns(timeProviderInForteenDaysResult);
            timeProvider.InSixtyDays().Returns(timeProviderInSixtyDaysResult);
            return timeProvider;
        }
        
        private Word CreateWord(LearningStage learningStage)
        {
            return new Word("", "", PartOfSpeech.Noun, learningStage, DateTime.Today, DateTime.Today);
        }
    }
}