using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public class CodeStructureViewFactory
    {
        private readonly IAdornmentSpaceReservation _spaceReservation;
        private readonly CodeStructureViewModel _viewModel;

        public CodeStructureViewFactory(IAdornmentSpaceReservation spaceReservation, CodeStructureViewModel viewModel)
        {
            _spaceReservation = spaceReservation ?? throw new System.ArgumentNullException(nameof(spaceReservation));
            _viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public CodeStructureView Create()
        {
            return new CodeStructureView(_viewModel, _spaceReservation);
        }
    }
}
