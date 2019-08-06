using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.ProcessDefinition;
using Xunit;

namespace Camunda.Api.Client.Tests
{
    public class ExternalTaskTests
    {
        [Fact]
        public async Task CompleteTest(){
            var client = CamundaClient.Create("http://localhost:8080/engine-rest");
            var processInstance = await client.ProcessDefinitions.ByKey("ExternalTaskTest")
                .StartProcessInstance(new StartProcessInstance());

            Assert.NotNull(processInstance);
            
            var externalTasks = await client.ExternalTasks.FetchAndLock(new FetchExternalTasks(){
                WorkerId = "DOT-NET-TEST",
                MaxTasks = 1,
                Topics = new List<FetchExternalTaskTopic>(){
                    new FetchExternalTaskTopic("external-task", 10000L)
                }
            });

            Assert.NotNull(externalTasks);
            Assert.True(externalTasks.Count > 0);
            
            externalTasks.ForEach(action: async externalTask => {
                await client.ExternalTasks.Complete(externalTask.Id, new CompleteExternalTask(){
                    WorkerId = "DOT-NET-TEST"
                });
            });
        }
    }
}